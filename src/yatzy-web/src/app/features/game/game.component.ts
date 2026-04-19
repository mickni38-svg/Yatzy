import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { GameRealtimeService } from '../../core/services/game-realtime.service';
import { PlayerSessionService } from '../../core/services/player-session.service';
import { WebRtcService } from '../../core/services/webrtc.service';
import { GameStateDto } from '../../core/models/game-state.dto';
import { GameStatus } from '../../shared/enums/game-status.enum';
import { ScoreCategory, ScoreCategoryLabel } from '../../shared/enums/score-category.enum';
import { DiceTileComponent } from '../../shared/components/dice-tile/dice-tile.component';
import { ScoreSheetComponent } from '../../shared/components/score-sheet/score-sheet.component';
import { PlayerListComponent } from '../../shared/components/player-list/player-list.component';

@Component({
  selector: 'app-game',
  standalone: true,
  imports: [CommonModule, DiceTileComponent, ScoreSheetComponent, PlayerListComponent],
  templateUrl: './game.component.html',
  styleUrl: './game.component.scss'
})
export class GameComponent implements OnInit, OnDestroy {
  game: GameStateDto | null = null;
  errorMessage = '';
  isActing = false;
  remoteStreams = new Map<string, MediaStream>();
  selectedCategory: ScoreCategory | null = null;

  readonly allCategories = [
    ScoreCategory.Ones, ScoreCategory.Twos, ScoreCategory.Threes,
    ScoreCategory.Fours, ScoreCategory.Fives, ScoreCategory.Sixes,
    ScoreCategory.OnePair, ScoreCategory.TwoPairs, ScoreCategory.ThreeOfAKind,
    ScoreCategory.FourOfAKind, ScoreCategory.SmallStraight, ScoreCategory.LargeStraight,
    ScoreCategory.FullHouse, ScoreCategory.Chance, ScoreCategory.Yatzy
  ];

  private sub = new Subscription();

  constructor(
    private realtime: GameRealtimeService,
    private session: PlayerSessionService,
    private router: Router,
    public webrtc: WebRtcService
  ) {}

  ngOnInit(): void {
    this.game = this.realtime.currentState;

    if (!this.session.hasSession()) {
      this.router.navigate(['/']);
      return;
    }

    this.sub.add(this.webrtc.remoteStreams$.subscribe(m => this.remoteStreams = new Map(m)));

    this.sub.add(
      this.realtime.gameState$.subscribe(state => {
        if (!state) return;
        this.game = state;
        this.selectedCategory = null;
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
    // Start kun WebRTC hvis det ikke allerede kører fra lobby
    if (playerId && roomCode && !this.webrtc.localStream) {
      this.webrtc.start(roomCode, playerId);
    }
  }

  async rollDice(): Promise<void> {
    if (!this.game || !this.isMyTurn || this.canSelectOnly) return;
    this.isActing = true;
    this.errorMessage = '';
    try {
      await this.realtime.rollDice(this.game.gameId, this.myPlayerId!);
    } finally {
      this.isActing = false;
    }
  }

  async onToggleHold(position: number): Promise<void> {
    if (!this.game || !this.isMyTurn || this.game.rollNumber === 0) return;
    await this.realtime.toggleHold(this.game.gameId, this.myPlayerId!, position);
  }

  selectCategoryPending(cat: ScoreCategory): void {
    if (this.isCategoryUsed(cat)) return;
    this.selectedCategory = this.selectedCategory === cat ? null : cat;
  }

  async confirmCategory(): Promise<void> {
    if (!this.selectedCategory) return;
    await this.onSelectCategory(this.selectedCategory);
    this.selectedCategory = null;
  }

  async onSelectCategory(category: ScoreCategory): Promise<void> {
    if (!this.game || !this.isMyTurn || this.game.rollNumber === 0) return;
    this.isActing = true;
    this.errorMessage = '';
    try {
      await this.realtime.selectScore(this.game.gameId, this.myPlayerId!, category);
    } finally {
      this.isActing = false;
    }
  }

  isCategoryAvailable(cat: ScoreCategory): boolean {
    const me = this.game?.players.find(p => p.playerId === this.myPlayerId);
    if (!me) return false;
    return !me.scoreEntries.find(e => e.category === cat)?.isUsed;
  }

  isCategoryUsed(cat: ScoreCategory): boolean {
    const me = this.game?.players.find(p => p.playerId === this.myPlayerId);
    return me?.scoreEntries.find(e => e.category === cat)?.isUsed ?? false;
  }

  getCategorySuggestion(cat: ScoreCategory): number {
    if (!this.canSelectOnly || this.diceValues.length !== 5) return 0;
    const sheet = (this.game?.players.find(p => p.playerId === this.myPlayerId)
      ?.scoreEntries ?? []);
    if (sheet.find(e => e.category === cat)?.isUsed) return 0;
    // delegate to score-sheet logic – use suggestion from scoreEntries if available
    return 0; // will be calculated in template via score-sheet component
  }

  categoryLabel(cat: ScoreCategory): string {
    return ScoreCategoryLabel[cat];
  }

  bestSuggestionPoints(): number {
    // returns total of current dice for display
    return this.diceValues.reduce((a, b) => a + b, 0);
  }

  get myPlayerId(): string | null { return this.session.playerId; }
  get isMyTurn(): boolean { return this.game?.currentPlayerId === this.myPlayerId; }
  get canRoll(): boolean { return this.isMyTurn && (this.game?.rollNumber ?? 0) < 3; }
  get canSelectOnly(): boolean { return this.isMyTurn && (this.game?.rollNumber ?? 0) >= 3; }

  get currentPlayerName(): string {
    return this.game?.players.find(p => p.playerId === this.game?.currentPlayerId)?.displayName ?? '';
  }

  get diceValues(): number[] {
    return this.game?.dice.map(d => d.value) ?? [];
  }

  ngOnDestroy(): void {
    this.sub.unsubscribe();
    this.webrtc.stop();
  }
}

