import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerDto } from '../../../core/models/game-state.dto';
import { SrcObjectDirective } from '../../directives/src-object.directive';

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [CommonModule, SrcObjectDirective],
  templateUrl: './player-list.component.html',
  styleUrl: './player-list.component.scss'
})
export class PlayerListComponent {
  @Input() players: PlayerDto[] = [];
  @Input() currentPlayerId: string | null = null;
  @Input() myPlayerId: string | null = null;
  @Input() streams: Map<string, MediaStream> = new Map();

  getStream(playerId: string): MediaStream | null {
    return this.streams.get(playerId) ?? null;
  }
}
