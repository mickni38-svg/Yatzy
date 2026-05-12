import { Component, OnInit, OnDestroy, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';
import { GameRealtimeService } from '../../core/services/game-realtime.service';
import { PlayerSessionService } from '../../core/services/player-session.service';
import { GameStateDto } from '../../core/models/game-state.dto';
import { GameStatus } from '../../shared/enums/game-status.enum';
import { ScoreCategory } from '../../shared/enums/score-category.enum';
import { DiceTileComponent } from '../../shared/components/dice-tile/dice-tile.component';
import { ScoreSheetComponent } from '../../shared/components/score-sheet/score-sheet.component';
import { WebRtcService } from '../../core/services/webrtc.service';
import { DiceSoundService } from '../../core/services/dice-sound.service';

export interface GifEntry {
  name: string;
  file: string;
  showOverlay?: boolean;
}

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, FormsModule, DiceTileComponent, ScoreSheetComponent],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent implements OnInit, OnDestroy {
  game: GameStateDto | null = null;
  errorMessage = '';
  isActing = false;
  remoteStreams = new Map<string, MediaStream>();
  allStreams = new Map<string, MediaStream>();

  /** Værdier vist på terningerne – cykler tilfældigt under spin-animation */
  animatedValues: number[] = [1, 1, 1, 1, 1];
  /** Hvilke terninger (indeks) der aktuelt ruller */
  rollingDice: boolean[] = [false, false, false, false, false];
  /** True mens animation er i gang – template bruger animatedValues for alle terninger */
  isAnimating = false;
  private rollInterval: ReturnType<typeof setInterval> | null = null;
  /** True mens mindst én terning stadig ruller – skjuler score-forslag */
  diceSpinning = false;
  /** Gemmer det endelige state fra DiceRolled mens spin-animation kører */
  private _pendingRolledState: GameStateDto | null = null;

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
  cameraEnabled = false;

  get cameraGridCols(): string {
    const n = this.game?.players.length ?? 0;
    return n <= 1 ? 'repeat(1, 1fr)' : 'repeat(2, 1fr)';
  }

  yatzyCelebrating = new Set<string>();
  yatzyCelebrationGif = new Map<string, string>();
  gifList: GifEntry[] = [];
  selectedGif: GifEntry | null = null;
  private yatzyTimers = new Map<string, ReturnType<typeof setTimeout>>();
  private previousYatzyPlayers = new Set<string>();

  toggleCamera(): void {
    this.cameraEnabled = !this.cameraEnabled;
  }

  /** Vis en specifik GIF lokalt på en spillers tile */
  private _showGif(playerId: string, gifFile: string): void {
    if (this.yatzyTimers.has(playerId)) {
      clearTimeout(this.yatzyTimers.get(playerId)!);
    }
    this.yatzyCelebrationGif = new Map(this.yatzyCelebrationGif).set(playerId, gifFile);
    this.yatzyCelebrating = new Set(this.yatzyCelebrating).add(playerId);
    const t = setTimeout(() => {
      const next = new Set(this.yatzyCelebrating);
      next.delete(playerId);
      this.yatzyCelebrating = next;
      this.yatzyTimers.delete(playerId);
    }, 6000);
    this.yatzyTimers.set(playerId, t);
  }

  /** Host klikker på knap: send valgt GIF til server → alle ser samme GIF */
  triggerYatzyCelebration(playerId: string, sendToServer = false): void {
    const gifFile = this.selectedGif?.file ?? 'yatzy-celebration.gif';
    if (sendToServer && this.game) {
      this.realtime.triggerYatzy(playerId, gifFile).catch(() => {/* best-effort */});
      // Vis lokalt med det samme
    }
    this._showGif(playerId, gifFile);
  }

  compareGifs(a: GifEntry, b: GifEntry): boolean {
    return a?.file === b?.file;
  }

  getCelebrationGif(playerId: string): string {
    return this.yatzyCelebrationGif.get(playerId) ?? 'yatzy-celebration.gif';
  }

  showYatzyOverlay(playerId: string): boolean {
    const file = this.yatzyCelebrationGif.get(playerId);
    if (!file) return false;
    return this.gifList.find(g => g.file === file)?.showOverlay ?? false;
  }

  isCelebrating(playerId: string): boolean {
    return this.yatzyCelebrating.has(playerId);
  }

  private sub = new Subscription();

  constructor(
    private realtime: GameRealtimeService,
    private session: PlayerSessionService,
    private router: Router,
    public webrtc: WebRtcService,
    private sound: DiceSoundService,
    private http: HttpClient,
    private zone: NgZone
  ) {}

  ngOnInit(): void {
    this.game = this.realtime.currentState;

    // Indlæs GIF-konfiguration
    this.http.get<GifEntry[]>('gif-config.json').subscribe({
      next: list => {
        this.gifList = list;
        this.selectedGif = list[0] ?? null;
      },
      error: () => {
        // Fallback hvis config ikke kan læses
        this.gifList = [{ name: '🍕 Pizza-jubel', file: 'yatzy-celebration.gif' }];
        this.selectedGif = this.gifList[0];
      }
    });

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

        // Detect om nogen netop har scoret Yatzy
        for (const player of state.players) {
          const hasYatzy = player.scoreEntries.some(
            e => e.category === 'Yatzy' && e.isUsed && (e.score ?? 0) > 0
          );
          const hadYatzyBefore = this.previousYatzyPlayers.has(player.playerId);
          if (hasYatzy && !hadYatzyBefore) {
            // Brug altid den første GIF i config som automatisk Yatzy-fejrings-GIF
            const gifFile = this.gifList[0]?.file ?? 'yatzy-celebration.gif';
            const isWinner = player.playerId === this.myPlayerId;
            const amHost = this.isIAmHost;
            // Vinderen broadcaster til alle — host broadcaster som fallback
            const delay = isWinner ? 0 : (amHost ? 300 : -1);
            if (delay >= 0) {
              setTimeout(() => {
                if (!this.isCelebrating(player.playerId)) {
                  this.realtime.triggerYatzy(player.playerId, gifFile).catch(() => {
                    this._showGif(player.playerId, gifFile);
                  });
                }
              }, delay);
            }
          }
          if (hasYatzy) this.previousYatzyPlayers.add(player.playerId);
        }

        this.game = state;
        this.hasSelectedCategory = false;

        if (state.status === GameStatus.Completed) {
          // Navigér til resultater – behold SignalR-forbindelsen så spilleren forbliver connected
          this.router.navigate(['/results']);
          return;
        }
      })
    );

    // Alle klienter: DiceRolling starter 3s spin-animation
    this.sub.add(
      this.realtime.diceRolling$.subscribe(rollingPositions => {
        this.zone.run(() => {
          if (!this.game) return;
          this._startSpin(rollingPositions);
        });
      })
    );

    // Alle klienter: DiceRolled ankommer mens spin kører – gem og reveal sekventielt
    this.sub.add(
      this.realtime.diceRolled$.subscribe(state => {
        this.zone.run(() => {
          this._pendingRolledState = state;
        });
      })
    );

    this.sub.add(
      this.realtime.error$.subscribe(msg => {
        this.errorMessage = msg;
        this.isActing = false;
      })
    );

    // Lytt til TriggerYatzy-events fra serveren (broadcast fra host)
    this.sub.add(
      this.realtime.yatzy$.subscribe(({ playerId, gifName }) => {
        this._showGif(playerId, gifName);
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
    this.errorMessage = '';
    try {
      await this.realtime.rollDice(this.game.gameId, this.myPlayerId!);
      // Animation starter via DiceRolling-event som serveren broadcaster til alle
    } catch {
      this.isActing = false;
    }
  }

  /**
   * Starter 3s spin-animation for de terninger der ruller (alle klienter).
   * Efter 3s afventer vi _pendingRolledState og revealer terningerne én ad gangen med 1s interval.
   */
  private _startSpin(rollingPositions: number[]): void {
    if (!this.game) return;

    // Indeks i dice-arrayet for de terninger der ruller
    const freeIndices = this.game.dice
      .map((d, i) => ({ d, i }))
      .filter(x => rollingPositions.includes(x.d.position))
      .map(x => x.i);

    if (freeIndices.length === 0) return;

    // Sæt startværdier og markér som rullende
    this.animatedValues = this.game.dice.map(d => d.value);
    this.rollingDice = this.game.dice.map(() => false);
    freeIndices.forEach(i => (this.rollingDice[i] = true));
    this.diceSpinning = true;
    this.isAnimating = true;
    this._pendingRolledState = null;

    // Start hurtig tilfældig animation
    if (this.rollInterval) clearInterval(this.rollInterval);
    this.rollInterval = setInterval(() => {
      freeIndices.forEach(i => {
        this.animatedValues[i] = Math.floor(Math.random() * 6) + 1;
      });
      this.animatedValues = [...this.animatedValues];
    }, 80);

    this.sound.startSpin();

    // Efter 3s: stop spin og reveal én terning ad gangen
    setTimeout(() => {
      if (this.rollInterval) { clearInterval(this.rollInterval); this.rollInterval = null; }
      this.sound.stopSpin();

      const finalState = this._pendingRolledState;
      const finalValues = finalState
        ? finalState.dice.map(d => d.value)
        : this.animatedValues.slice();

      freeIndices.forEach((dieIndex, order) => {
        setTimeout(() => {
          this.animatedValues[dieIndex] = finalValues[dieIndex];
          this.animatedValues = [...this.animatedValues];
          this.rollingDice[dieIndex] = false;
          this.rollingDice = [...this.rollingDice];
          this.sound.playBing();
        }, order * 1000);
      });

      const totalRevealMs = freeIndices.length * 1000;
      setTimeout(() => {
        this.diceSpinning = false;
        this.isAnimating = false;
        this.isActing = false;
        if (finalState) {
          // Opdater game state nu hvor animation er færdig
          this.realtime.applyState(finalState);
          this._pendingRolledState = null;
        }
        this._checkYatzyOnDice();
      }, totalRevealMs);
    }, 3000);
  }

  /** Tjek om alle 5 terninger (inkl. holdte) viser samme øjne → Yatzy */
  private _checkYatzyOnDice(): void {
    if (!this.game || !this.isMyTurn) return;

    const me = this.game.players.find(p => p.playerId === this.myPlayerId);
    if (!me) return;
    const yatzyAlreadyUsed = me.scoreEntries.some(
      e => e.category === 'Yatzy' && e.isUsed
    );
    if (yatzyAlreadyUsed) return;

    const vals = this.game.dice.map((d, i) =>
      this.rollingDice[i] ? this.animatedValues[i] : d.value
    );
    if (vals.length !== 5) return;
    const allSame = vals.every(v => v === vals[0]);
    if (!allSame) return;

    const gifFile = this.gifList[0]?.file ?? 'yatzy-celebration.gif';
    this.realtime.triggerYatzy(this.myPlayerId!, gifFile).catch(() => {
      this._showGif(this.myPlayerId!, gifFile);
    });
  }

  async onToggleHold(position: number): Promise<void> {
    if (!this.game || !this.isMyTurn || this.game.rollNumber === 0) return;
    await this.realtime.toggleHold(this.game.gameId, this.myPlayerId!, position);
  }

  async onSelectCategory(category: ScoreCategory): Promise<void> {
    if (!this.game || !this.isMyTurn || (this.game.rollNumber ?? 0) === 0) return;
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
  get isIAmHost(): boolean { return !!this.myPlayerId && this.isHost(this.myPlayerId); }
  get canRoll(): boolean { return this.isMyTurn && (this.game?.rollNumber ?? 0) < 3; }
  get canSelectOnly(): boolean { return (this.game?.rollNumber ?? 0) >= 3; }
  get canSelectEarly(): boolean { return this.isMyTurn && (this.game?.rollNumber ?? 0) > 0 && (this.game?.rollNumber ?? 0) < 3; }
  isHost(playerId: string): boolean { return this.game?.players[0]?.playerId === playerId; }

  async leaveGame(): Promise<void> {
    const gameId = this.game?.gameId;
    const playerId = this.myPlayerId;

    // Afmeld subscription så inkommende state-opdateringer ikke forstyrrer
    this.sub.unsubscribe();

    // Stop kamera og mikrofon
    await this.webrtc.stop();

    try {
      if (gameId && playerId) {
        await this.realtime.leaveGame(gameId, playerId);
      }
    } catch {
      // best-effort
    }

    // Stop SignalR-forbindelsen
    try { await this.realtime.stop(); } catch { /* best-effort */ }

    this.router.navigate(['/']);
  }

  get currentPlayerName(): string {
    return this.game?.players.find(p => p.playerId === this.game?.currentPlayerId)?.displayName ?? '';
  }

  get activePlayers() {
    return this.game?.players.filter(p => !p.hasLeft) ?? [];
  }

  get diceValues(): number[] {
    return this.game?.dice.map(d => d.value) ?? [];
  }

  get diceTotal(): number {
    return this.diceValues.reduce((a, b) => a + b, 0);
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    if (this.rollInterval) { clearInterval(this.rollInterval); this.rollInterval = null; }
    this.sound.stopSpin();
    this.yatzyTimers.forEach(t => clearTimeout(t));
    this.yatzyTimers.clear();
    // webrtc.stop() er allerede kaldt i leaveGame() — kald kun hvis stream stadig kører
    if (this.webrtc.localStream) {
      this.webrtc.stop();
    }
  }
}

