import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class DiceSoundService {
  private ctx: AudioContext | null = null;
  private spinNode: AudioBufferSourceNode | null = null;
  private spinGain: GainNode | null = null;

  private getCtx(): AudioContext {
    if (!this.ctx) this.ctx = new AudioContext();
    return this.ctx;
  }

  /** Start rullende terningelyd (hvid støj med rytmisk tremolo) */
  startSpin(): void {
    try {
      const ctx = this.getCtx();
      if (ctx.state === 'suspended') ctx.resume();

      // Hvid støj buffer (0.5 s, loopes)
      const bufLen = ctx.sampleRate * 0.5;
      const buf = ctx.createBuffer(1, bufLen, ctx.sampleRate);
      const data = buf.getChannelData(0);
      for (let i = 0; i < bufLen; i++) data[i] = (Math.random() * 2 - 1) * 0.35;

      // Bandpass filter så det lyder som ræklende terninger
      const filter = ctx.createBiquadFilter();
      filter.type = 'bandpass';
      filter.frequency.value = 420;
      filter.Q.value = 0.8;

      // Tremolo (LFO) for at give rytmisk rullelyd
      const lfo = ctx.createOscillator();
      lfo.frequency.value = 9; // 9 Hz rytme
      const lfoGain = ctx.createGain();
      lfoGain.gain.value = 0.45;
      lfo.connect(lfoGain);

      this.spinGain = ctx.createGain();
      this.spinGain.gain.value = 0.55;
      lfoGain.connect(this.spinGain.gain); // tremolo på master gain

      this.spinNode = ctx.createBufferSource();
      this.spinNode.buffer = buf;
      this.spinNode.loop = true;
      this.spinNode.connect(filter);
      filter.connect(this.spinGain);
      this.spinGain.connect(ctx.destination);

      lfo.start();
      this.spinNode.start();
    } catch { /* ignore AudioContext errors */ }
  }

  /** Fade ud og stop snurrelyden */
  stopSpin(): void {
    try {
      if (!this.spinGain || !this.spinNode || !this.ctx) return;
      const now = this.ctx.currentTime;
      this.spinGain.gain.setValueAtTime(this.spinGain.gain.value, now);
      this.spinGain.gain.linearRampToValueAtTime(0, now + 0.25);
      const node = this.spinNode;
      setTimeout(() => { try { node.stop(); } catch { /* ok */ } }, 300);
      this.spinNode = null;
      this.spinGain = null;
    } catch { /* ignore */ }
  }

  /** Kort "bing" lyd når en terning stopper */
  playBing(): void {
    try {
      const ctx = this.getCtx();
      if (ctx.state === 'suspended') ctx.resume();

      const osc = ctx.createOscillator();
      const gain = ctx.createGain();
      osc.type = 'sine';
      osc.frequency.value = 880;
      gain.gain.setValueAtTime(0.55, ctx.currentTime);
      gain.gain.exponentialRampToValueAtTime(0.001, ctx.currentTime + 0.38);
      osc.connect(gain);
      gain.connect(ctx.destination);
      osc.start(ctx.currentTime);
      osc.stop(ctx.currentTime + 0.4);
    } catch { /* ignore */ }
  }
}
