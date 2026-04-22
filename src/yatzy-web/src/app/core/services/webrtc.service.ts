import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';

const VIDEO_HUB_URL = environment.hubUrl.replace('/hubs/game', '/hubs/video');

const ICE_SERVERS: RTCIceServer[] = [
  { urls: 'stun:stun.l.google.com:19302' },
  { urls: 'stun:stun1.l.google.com:19302' },
  {
    urls: 'turn:openrelay.metered.ca:80',
    username: 'openrelayproject',
    credential: 'openrelayproject'
  },
  {
    urls: 'turn:openrelay.metered.ca:443',
    username: 'openrelayproject',
    credential: 'openrelayproject'
  },
  {
    urls: 'turn:openrelay.metered.ca:443?transport=tcp',
    username: 'openrelayproject',
    credential: 'openrelayproject'
  },
];

@Injectable({ providedIn: 'root' })
export class WebRtcService implements OnDestroy {
  /** Map of playerId → remote MediaStream */
  readonly remoteStreams$ = new BehaviorSubject<Map<string, MediaStream>>(new Map());

  /** Own camera/mic stream */
  localStream: MediaStream | null = null;

  private connection: signalR.HubConnection | null = null;
  private peers = new Map<string, RTCPeerConnection>(); // connectionId → pc
  private peerPlayerIds = new Map<string, string>();    // connectionId → playerId
  // ICE candidates that arrived before remoteDescription was set
  private iceCandidateQueue = new Map<string, RTCIceCandidateInit[]>();

  // ── Public API ─────────────────────────────────────────────────────────────

  async start(roomCode: string, playerId: string): Promise<void> {
    // 1. Get camera + mic
    try {
      this.localStream = await navigator.mediaDevices.getUserMedia({
        video: { width: 320, height: 180, frameRate: 15 },
        audio: true
      });
    } catch {
      console.warn('[WebRTC] No camera/mic access – streaming disabled.');
      this.localStream = null;
    }

    // 2. Connect signaling hub
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(VIDEO_HUB_URL, { withCredentials: true })
      .withAutomaticReconnect()
      .build();

    this.connection.on('PeerJoined', (peerId: string, connId: string) =>
      this.onPeerJoined(peerId, connId));

    this.connection.on('ReceiveOffer', (sdp: string, peerId: string, connId: string) =>
      this.onReceiveOffer(sdp, peerId, connId));

    this.connection.on('ReceiveAnswerFrom', (sdp: string, connId: string) =>
      this.onReceiveAnswer(sdp, connId));

    this.connection.on('ReceiveIceCandidate',
      (candidate: string, mid: string, idx: number, connId: string) =>
        this.onReceiveIceCandidate(candidate, mid, idx, connId));

    this.connection.on('PeerLeft', (peerId: string) => this.onPeerLeft(peerId));

    await this.connection.start();
    await this.connection.invoke('JoinVideoRoom', roomCode, playerId);
  }

  async stop(): Promise<void> {
    this.peers.forEach(pc => pc.close());
    this.peers.clear();
    this.peerPlayerIds.clear();
    this.iceCandidateQueue.clear();
    this.localStream?.getTracks().forEach(t => t.stop());
    this.localStream = null;
    this.remoteStreams$.next(new Map());
    await this.connection?.stop();
    this.connection = null;
  }

  ngOnDestroy(): void { this.stop(); }

  // ── Internal ────────────────────────────────────────────────────────────────

  private async onPeerJoined(peerId: string, connId: string): Promise<void> {
    const pc = this.createPeerConnection(connId, peerId);
    const offer = await pc.createOffer();
    await pc.setLocalDescription(offer);
    await this.connection!.invoke('SendOffer', connId, offer.sdp, peerId);
  }

  private async onReceiveOffer(sdp: string, peerId: string, connId: string): Promise<void> {
    const pc = this.createPeerConnection(connId, peerId);
    await pc.setRemoteDescription({ type: 'offer', sdp });
    await this.flushIceQueue(connId, pc);
    const answer = await pc.createAnswer();
    await pc.setLocalDescription(answer);
    await this.connection!.invoke('SendAnswerTo', connId, answer.sdp, peerId);
  }

  private async onReceiveAnswer(sdp: string, connId: string): Promise<void> {
    const pc = this.peers.get(connId);
    if (pc) {
      await pc.setRemoteDescription({ type: 'answer', sdp });
      await this.flushIceQueue(connId, pc);
    }
  }

  private async onReceiveIceCandidate(
    candidate: string, mid: string, idx: number, connId: string
  ): Promise<void> {
    const pc = this.peers.get(connId);
    const init: RTCIceCandidateInit = { candidate, sdpMid: mid, sdpMLineIndex: idx };
    if (!pc || !pc.remoteDescription) {
      // Queue until remoteDescription is set
      const queue = this.iceCandidateQueue.get(connId) ?? [];
      queue.push(init);
      this.iceCandidateQueue.set(connId, queue);
    } else {
      await pc.addIceCandidate(new RTCIceCandidate(init)).catch(() => {});
    }
  }

  private async flushIceQueue(connId: string, pc: RTCPeerConnection): Promise<void> {
    const queued = this.iceCandidateQueue.get(connId) ?? [];
    this.iceCandidateQueue.delete(connId);
    for (const init of queued) {
      await pc.addIceCandidate(new RTCIceCandidate(init)).catch(() => {});
    }
  }

  private onPeerLeft(peerId: string): void {
    for (const [connId, pid] of this.peerPlayerIds) {
      if (pid === peerId) {
        this.peers.get(connId)?.close();
        this.peers.delete(connId);
        this.peerPlayerIds.delete(connId);
        this.iceCandidateQueue.delete(connId);
        break;
      }
    }
    const map = new Map(this.remoteStreams$.value);
    map.delete(peerId);
    this.remoteStreams$.next(map);
  }

  private createPeerConnection(connId: string, peerId: string): RTCPeerConnection {
    const pc = new RTCPeerConnection({ iceServers: ICE_SERVERS });
    this.peers.set(connId, pc);
    this.peerPlayerIds.set(connId, peerId);

    // Add local tracks
    this.localStream?.getTracks().forEach(t =>
      pc.addTrack(t, this.localStream!));

    // Receive remote track
    pc.ontrack = ({ streams }) => {
      if (streams[0]) {
        const map = new Map(this.remoteStreams$.value);
        map.set(peerId, streams[0]);
        this.remoteStreams$.next(map);
      }
    };

    // ICE
    pc.onicecandidate = ({ candidate }) => {
      if (candidate) {
        this.connection?.invoke('SendIceCandidate', connId,
          candidate.candidate, candidate.sdpMid ?? '', candidate.sdpMLineIndex ?? 0);
      }
    };

    return pc;
  }
}
