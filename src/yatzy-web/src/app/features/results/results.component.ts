import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { GameRealtimeService } from '../../core/services/game-realtime.service';
import { PlayerSessionService } from '../../core/services/player-session.service';
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

  constructor(
    private realtime: GameRealtimeService,
    private session: PlayerSessionService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.game = this.realtime.currentState;
    if (!this.game) { this.router.navigate(['/']); return; }
    this.standings = [...this.game.players].sort((a, b) => b.totalScore - a.totalScore);
  }

  get winner(): PlayerDto | null { return this.standings[0] ?? null; }

  get myPlayerId(): string | null { return this.session.playerId; }

  get isWinner(): boolean {
    return this.winner?.playerId === this.session.playerId;
  }

  playAgain(): void {
    this.session.clear();
    this.realtime.stop();
    this.router.navigate(['/']);
  }
}
