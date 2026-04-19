import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DiceTileComponent } from '../dice-tile/dice-tile.component';
import { DiceDto } from '../../../core/models/game-state.dto';

@Component({
  selector: 'app-dice-tray',
  standalone: true,
  imports: [CommonModule, DiceTileComponent],
  templateUrl: './dice-tray.component.html',
  styleUrl: './dice-tray.component.scss'
})
export class DiceTrayComponent {
  @Input() dice: DiceDto[] = [];
  @Input() canInteract = false;
  @Output() toggleHold = new EventEmitter<number>();
}
