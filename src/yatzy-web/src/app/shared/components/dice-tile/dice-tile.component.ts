import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

type PipKey = 'tl'|'tr'|'ml'|'mr'|'bl'|'br'|'c';

const PIP_MAP: Record<number, PipKey[]> = {
  1: ['c'],
  2: ['tl','br'],
  3: ['tl','c','br'],
  4: ['tl','tr','bl','br'],
  5: ['tl','tr','c','bl','br'],
  6: ['tl','tr','ml','mr','bl','br'],
};

@Component({
  selector: 'app-dice-tile',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dice-tile.component.html',
  styleUrl: './dice-tile.component.scss'
})
export class DiceTileComponent {
  @Input() value = 1;
  @Input() isHeld = false;
  @Input() disabled = false;
  @Input() isRolling = false;
  @Output() toggle = new EventEmitter<void>();

  showPip(key: PipKey): boolean {
    return (PIP_MAP[this.value] ?? []).includes(key);
  }

  onToggle(): void {
    if (!this.disabled) this.toggle.emit();
  }
}
