import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-turn-banner',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './turn-banner.component.html',
  styleUrl: './turn-banner.component.scss'
})
export class TurnBannerComponent {
  @Input() currentPlayerName = '';
  @Input() isMyTurn = false;
  @Input() roundNumber = 1;
  @Input() rollNumber = 0;
  @Input() maxRolls = 3;
}
