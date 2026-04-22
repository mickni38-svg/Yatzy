import { Component, Input, OnChanges, AfterViewChecked, ElementRef, ViewChildren, QueryList } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerDto } from '../../../core/models/game-state.dto';

@Component({
  selector: 'app-player-list',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './player-list.component.html',
  styleUrl: './player-list.component.scss'
})
export class PlayerListComponent implements AfterViewChecked {
  @Input() players: PlayerDto[] = [];
  @Input() currentPlayerId: string | null = null;
  @Input() myPlayerId: string | null = null;
  @Input() streams: Map<string, MediaStream> = new Map();

  @ViewChildren('videoEl') videoEls!: QueryList<ElementRef<HTMLVideoElement>>;

  getStream(playerId: string): MediaStream | null {
    return this.streams.get(playerId) ?? null;
  }

  ngAfterViewChecked(): void {
    this.videoEls?.forEach(ref => {
      const el = ref.nativeElement;
      const playerId = el.dataset['playerId'];
      if (!playerId) return;
      const stream = this.streams.get(playerId) ?? null;
      if (el.srcObject !== stream) {
        el.srcObject = stream;
      }
    });
  }
}
