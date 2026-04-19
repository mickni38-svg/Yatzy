import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { GameApiService } from '../../core/services/game-api.service';
import { GameRealtimeService } from '../../core/services/game-realtime.service';
import { PlayerSessionService } from '../../core/services/player-session.service';
import { WebRtcService } from '../../core/services/webrtc.service';
import { GameStateDto } from '../../core/models/game-state.dto';
import { GameStatus } from '../../shared/enums/game-status.enum';
import { ScoreSheetComponent } from '../../shared/components/score-sheet/score-sheet.component';
import { SrcObjectDirective } from '../../shared/directives/src-object.directive';

@Component({
  selector: 'app-lobby',
  standalone: true,
  imports: [CommonModule, FormsModule, ScoreSheetComponent, SrcObjectDirective],
  templateUrl: './lobby.component.html',
  styleUrl: './lobby.component.scss'
})
export class LobbyComponent implements OnInit, OnDestroy {
  mode: 'home' | 'create' | 'join' | 'waiting' = 'home';
  hostName = '';
  playerName = '';
  roomCodeInput = '';
  errorMessage = '';
  isLoading = false;

  game: GameStateDto | null = null;
  remoteStreams = new Map<string, MediaStream>();

  private sub = new Subscription();

  constructor(
    private api: GameApiService,
    private realtime: GameRealtimeService,
    public session: PlayerSessionService,
    public webrtc: WebRtcService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.sub.add(
      this.webrtc.remoteStreams$.subscribe(m => this.remoteStreams = new Map(m))
    );

    this.sub.add(
      this.realtime.gameState$.subscribe(state => {
        if (!state) return;
        this.game = state;
        if (state.status === GameStatus.InProgress) {
          this.router.navigate(['/game']);
        }
      })
    );

    this.sub.add(
      this.realtime.error$.subscribe(msg => this.errorMessage = msg)
    );
  }

  async createGame(): Promise<void> {
    if (!this.hostName.trim()) { this.errorMessage = 'Name is required.'; return; }
    this.isLoading = true;
    this.errorMessage = '';
    try {
      this.session.clear();
      const state = await this.api.createGame({ hostName: this.hostName.trim() }).toPromise();
      if (!state) return;
      const host = state.players[0];
      this.session.save(host.playerId, state.gameId, state.roomCode);
      await this.realtime.start();
      await this.realtime.joinRoom(state.roomCode, host.playerId);
      this.game = state;
      this.mode = 'waiting';
      // Start kamera allerede i lobby
      this.webrtc.start(state.roomCode, host.playerId);
    } catch {
      this.errorMessage = 'Could not create game.';
    } finally {
      this.isLoading = false;
    }
  }

  async joinGame(): Promise<void> {
    if (!this.playerName.trim() || !this.roomCodeInput.trim()) {
      this.errorMessage = 'Name and room code are required.';
      return;
    }
    this.isLoading = true;
    this.errorMessage = '';
    try {
      this.session.clear();
      const state = await this.api.joinGame(this.roomCodeInput.trim().toUpperCase(), { playerName: this.playerName.trim() }).toPromise();
      if (!state) return;
      const me = state.players.find(p => p.displayName === this.playerName.trim());
      if (!me) return;
      this.session.save(me.playerId, state.gameId, state.roomCode);
      await this.realtime.start();
      await this.realtime.joinRoom(state.roomCode, me.playerId);
      this.game = state;
      this.mode = 'waiting';
      // Start kamera allerede i lobby
      this.webrtc.start(state.roomCode, me.playerId);
    } catch {
      this.errorMessage = 'Could not join game. Check the room code.';
    } finally {
      this.isLoading = false;
    }
  }

  async startGame(): Promise<void> {
    if (!this.game) return;
    const session = this.session;
    if (!session.playerId) return;
    this.isLoading = true;
    this.errorMessage = '';
    try {
      await this.realtime.startGame(this.game.gameId, session.playerId);
    } catch {
      this.errorMessage = 'Could not start game.';
    } finally {
      this.isLoading = false;
    }
  }

  get isHost(): boolean {
    if (!this.game || !this.session.playerId) return false;
    return this.game.players[0]?.playerId === this.session.playerId;
  }

  get canStart(): boolean {
    return !!this.game && this.game.players.length >= 2 && this.isHost;
  }

  get myPlayerId(): string | null {
    return this.session.playerId;
  }

  getStream(playerId: string): MediaStream | null {
    return this.remoteStreams.get(playerId) ?? null;
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    // WebRTC stopper IKKE her – game-komponenten overtager den samme session
  }
}
