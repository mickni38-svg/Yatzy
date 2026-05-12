import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GameRealtimeService } from '../../core/services/game-realtime.service';
import { PlayerSessionService } from '../../core/services/player-session.service';
import { GameApiService } from '../../core/services/game-api.service';
import { GameStateDto, PlayerDto } from '../../core/models/game-state.dto';

@Component({
  selector: 'app-results',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './results.component.html',
  styleUrl: './results.component.scss'
})
export class ResultsComponent implements OnInit {
  game: GameStateDto | null = null;
  standings: PlayerDto[] = [];
  isCreating = false;
  errorMessage = '';

  constructor(
    private realtime: GameRealtimeService,
    private session: PlayerSessionService,
    private api: GameApiService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.game = this.realtime.currentState;
    if (!this.game) { this.router.navigate(['/']); return; }
    this.standings = [...this.game.players].sort((a, b) => b.totalScore - a.totalScore);
  }

  get winner(): PlayerDto | null { return this.standings[0] ?? null; }
  get myPlayerId(): string | null { return this.session.playerId; }
  get isWinner(): boolean { return this.winner?.playerId === this.session.playerId; }

  get myDisplayName(): string {
    return this.game?.players.find(p => p.playerId === this.myPlayerId)?.displayName ?? '';
  }

  /** Opret et nyt spil med samme navn – session bevares (samme playerId gemmes efter oprettelse) */
  async newGame(): Promise<void> {
    const name = this.myDisplayName;
    if (!name) { this.router.navigate(['/']); return; }
    this.isCreating = true;
    this.errorMessage = '';
    try {
      await this.realtime.stop();
      const state = await this.api.createGame({ hostName: name }).toPromise();
      if (!state) return;
      const host = state.players[0];
      this.session.save(host.playerId, state.gameId, state.roomCode);
      await this.realtime.start();
      await this.realtime.joinRoom(state.roomCode, host.playerId);
      this.router.navigate(['/lobby']);
    } catch {
      this.errorMessage = 'Kunne ikke oprette nyt spil.';
      this.isCreating = false;
    }
  }

  /** Tilbage til forsiden uden at rydde navn */
  goHome(): void {
    this.router.navigate(['/']);
  }
}
