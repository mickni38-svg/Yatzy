import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { GameRealtimeService } from '../../core/services/game-realtime.service';
import { PlayerSessionService } from '../../core/services/player-session.service';
import { GameStateDto, DiceDto } from '../../core/models/game-state.dto';
import { GameStatus } from '../../shared/enums/game-status.enum';
import { ScoreCategory } from '../../shared/enums/score-category.enum';
import { DiceTileComponent } from '../../shared/components/dice-tile/dice-tile.component';
import { ScoreSheetComponent } from '../../shared/components/score-sheet/score-sheet.component';
import { WebRtcService } from '../../core/services/webrtc.service';
import { DiceSoundService } from '../../core/services/dice-sound.service';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, DiceTileComponent, ScoreSheetComponent],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent implements OnInit, OnDestroy {
  game: GameStateDto | null = null;
  errorMessage = '';
  isActing = false;
  remoteStreams = new Map<string, MediaStream>();
  allStreams = new Map<string, MediaStream>();

  /** Værdier vist på terningerne – cykler tilfældigt under animation */
  animatedValues: number[] = [1, 1, 1, 1, 1];
  /** Hvilke terninger (indeks) der aktuelt ruller */
  rollingDice: boolean[] = [false, false, false, false, false];
  private rollInterval: ReturnType<typeof setInterval> | null = null;
  /** Bruges til at undgå double-trigger: sæt til true mens DEN lokale spiller kaster */
  private isLocallyRolling = false;
  private previousRollNumber = 0;

  private updateAllStreams(remote: Map<string, MediaStream>): void {
    const map = new Map(remote);
    const myId = this.myPlayerId;
    if (myId && this.webrtc.localStream) {
      map.set(myId, this.webrtc.localStream);
    }
    this.allStreams = map;
  }
  hasSelectedCategory = false;
  showScore = false;
  cameraEnabled = true;

  toggleCamera(): void {
    this.cameraEnabled = !this.cameraEnabled;
  }

  private sub = new Subscription();

  constructor(
    private realtime: GameRealtimeService,
    private session: PlayerSessionService,
    private router: Router,
    public webrtc: WebRtcService,
    private sound: DiceSoundService
  ) {}

  ngOnInit(): void {
    this.game = this.realtime.currentState;

    if (!this.session.hasSession()) {
      this.router.navigate(['/']);
      return;
    }

    this.sub.add(this.webrtc.remoteStreams$.subscribe(m => {
      this.remoteStreams = new Map(m);
      this.updateAllStreams(this.remoteStreams);
    }));

    this.sub.add(
      this.realtime.gameState$.subscribe(state => {
        if (!state) return;

        const prevRoll = this.previousRollNumber;
        this.previousRollNumber = state.rollNumber;

        this.game = state;
        this.hasSelectedCategory = false;

        // En anden spiller har kastet – trigger animation for tilskuere
        if (state.rollNumber > 0 && state.rollNumber !== prevRoll && !this.isLocallyRolling) {
          this._triggerAnimation(state.dice, false);
        }

        if (state.status === GameStatus.Completed) {
          this.router.navigate(['/results']);
        }
      })
    );

    this.sub.add(
      this.realtime.error$.subscribe(msg => {
        this.errorMessage = msg;
        this.isActing = false;
      })
    );

    const playerId = this.session.playerId;
    const roomCode = this.session.roomCode;
    if (playerId && roomCode && !this.webrtc.localStream) {
      this.webrtc.start(roomCode, playerId).then(() => {
        this.updateAllStreams(this.remoteStreams);
      });
    } else {
      this.updateAllStreams(this.remoteStreams);
    }
  }

  async rollDice(): Promise<void> {
    if (!this.game || !this.isMyTurn || this.canSelectOnly) return;
    this.isActing = true;
    this.isLocallyRolling = true;
    this.errorMessage = '';

    this._triggerAnimation(this.game.dice, true);

    try {
      await Promise.all([
        this.realtime.rollDice(this.game.gameId, this.myPlayerId!),
        new Promise<void>(res => setTimeout(res, 3000 + 5 * 1000 + 200)) // 3s spin + 5×1s stop + buffer
      ]);
    } catch {
      this._stopRollAnimation();
      this.isActing = false;
      this.isLocallyRolling = false;
    }
  }

  /**
   * Fælles animationslogik for båden kasteren og tilskuere.
   * isRoller = true  → styrer isActing + isLocallyRolling efter stop
   * isRoller = false → tilskuer: 3 sekunders animation, derefter stop
   */
  private _triggerAnimation(dice: DiceDto[], isRoller: boolean): void {
    const freeIndices = dice
      .map((d, i) => ({ d, i }))
      .filter(x => !x.d.isHeld)
      .map(x => x.i);

    if (freeIndices.length === 0) {
      if (isRoller) { this.isActing = false; this.isLocallyRolling = false; }
      return;
    }

    this.animatedValues = dice.map(d => d.value);
    this.rollingDice = dice.map(() => false);
    freeIndices.forEach(i => (this.rollingDice[i] = true));

    if (this.rollInterval) clearInterval(this.rollInterval);
    this.rollInterval = setInterval(() => {
      freeIndices.forEach(i => {
        this.animatedValues[i] = Math.floor(Math.random() * 6) + 1;
      });
      this.animatedValues = [...this.animatedValues];
    }, 80);

    if (isRoller) this.sound.startSpin();

    // Kasteren: 3s spin, derefter stop en efter en med 1s interval
    // Tilskuere: 3s spin, derefter alle terninger stopper med det samme
    const stopDelay = isRoller ? 3000 : 3000;
    const stopInterval = isRoller ? 1000 : 0;
    setTimeout(() => {
      if (this.rollInterval) { clearInterval(this.rollInterval); this.rollInterval = null; }
      if (isRoller) this.sound.stopSpin();

      freeIndices.forEach((dieIndex, order) => {
        setTimeout(() => {
          this.rollingDice[dieIndex] = false;
          this.rollingDice = [...this.rollingDice];
          if (isRoller) this.sound.playBing();
        }, order * stopInterval);
      });

      setTimeout(() => {
        if (isRoller) { this.isActing = false; this.isLocallyRolling = false; }
      }, freeIndices.length * stopInterval);
    }, stopDelay);
  }

  private _stopRollAnimation(): void {
    if (this.rollInterval) { clearInterval(this.rollInterval); this.rollInterval = null; }
    this.sound.stopSpin();
    this.rollingDice = [false, false, false, false, false];
    this.isLocallyRolling = false;
  }

  async onToggleHold(position: number): Promise<void> {
    if (!this.game || !this.isMyTurn || this.game.rollNumber === 0) return;
    await this.realtime.toggleHold(this.game.gameId, this.myPlayerId!, position);
  }

  async onSelectCategory(category: ScoreCategory): Promise<void> {
    if (!this.game || !this.isMyTurn || this.game.rollNumber === 0) return;
    // Tilføjet: marker at kategori er valgt, så Hold-knapper deaktiveres
    this.hasSelectedCategory = true;
    this.isActing = true;
    this.errorMessage = '';
    try {
      await this.realtime.selectScore(this.game.gameId, this.myPlayerId!, category);
    } finally {
      this.isActing = false;
    }
  }

  get myPlayerId(): string | null { return this.session.playerId; }
  get myDisplayName(): string { return this.game?.players.find(p => p.playerId === this.myPlayerId)?.displayName ?? 'Spiller'; }
  get isMyTurn(): boolean { return this.game?.currentPlayerId === this.myPlayerId; }
  get canRoll(): boolean { return this.isMyTurn && (this.game?.rollNumber ?? 0) < 3; }
  get canSelectOnly(): boolean { return (this.game?.rollNumber ?? 0) >= 3; }
  isHost(playerId: string): boolean { return this.game?.players[0]?.playerId === playerId; }

  /** CSS grid-template-columns baseret på antal spillere */
  get cameraGridCols(): string {
    const n = this.game?.players.length ?? 0;
    if (n <= 1) return 'repeat(1, 1fr)';
    if (n === 2) return 'repeat(2, 1fr)';
    return 'repeat(2, 1fr)'; // 3-4 spillere: 2×2
  }

  /** CSS grid-template-rows baseret på antal spillere */
  get cameraGridRows(): string {
    const n = this.game?.players.length ?? 0;
    if (n <= 2) return 'repeat(1, 1fr)';
    return 'repeat(2, 1fr)'; // 3-4 spillere: 2×2
  }

  async leaveGame(): Promise<void> {
    const gameId = this.game?.gameId;
    const playerId = this.myPlayerId;
    this.webrtc.stop();
    try {
      if (gameId && playerId) {
        await this.realtime.leaveGame(gameId, playerId);
      }
    } catch {
      // best-effort
    }
    window.close();
    // Fallback: hvis siden ikke kan lukkes (ikke åbnet via script), naviger til startsiden
    this.router.navigate(['/']);
  }

  get currentPlayerName(): string {
    return this.game?.players.find(p => p.playerId === this.game?.currentPlayerId)?.displayName ?? '';
  }

  get diceValues(): number[] {
    return this.game?.dice.map(d => d.value) ?? [];
  }

  get diceTotal(): number {
    return this.diceValues.reduce((a, b) => a + b, 0);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    this._stopRollAnimation();
    this.webrtc.stop();
  }
}

