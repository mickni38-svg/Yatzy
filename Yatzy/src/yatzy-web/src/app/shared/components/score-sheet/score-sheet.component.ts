import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PlayerDto } from '../../../core/models/game-state.dto';
import { ScoreCategory, ScoreCategoryLabel } from '../../enums/score-category.enum';

@Component({
  selector: 'app-score-sheet',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './score-sheet.component.html',
  styleUrl: './score-sheet.component.scss'
})
export class ScoreSheetComponent {
  @Input() players: PlayerDto[] = [];
  @Input() myPlayerId: string | null = null;
  @Input() canSelect = false;
  @Input() diceValues: number[] = [];
  @Output() selectCategory = new EventEmitter<ScoreCategory>();

  readonly upperCategories = [
    ScoreCategory.Ones, ScoreCategory.Twos, ScoreCategory.Threes,
    ScoreCategory.Fours, ScoreCategory.Fives, ScoreCategory.Sixes
  ];
  readonly lowerCategories = [
    ScoreCategory.OnePair, ScoreCategory.TwoPairs, ScoreCategory.ThreeOfAKind,
    ScoreCategory.FourOfAKind, ScoreCategory.SmallStraight, ScoreCategory.LargeStraight,
    ScoreCategory.FullHouse, ScoreCategory.Chance, ScoreCategory.Yatzy
  ];
  readonly allCategories = [...this.upperCategories, ...this.lowerCategories];
  readonly labels = ScoreCategoryLabel;

  getScore(player: PlayerDto, category: ScoreCategory): string {
    const entry = player.scoreEntries.find(e => e.category === category);
    return entry?.isUsed ? String(entry.score ?? 0) : '';
  }

  isUsed(player: PlayerDto, category: ScoreCategory): boolean {
    return player.scoreEntries.find(e => e.category === category)?.isUsed ?? false;
  }

  isMyCategory(player: PlayerDto, category: ScoreCategory): boolean {
    return player.playerId === this.myPlayerId && this.canSelect && !this.isUsed(player, category);
  }

  getSuggestion(category: ScoreCategory): number | null {
    if (!this.canSelect || this.diceValues.length !== 5) return null;
    return this.calcScore(category, this.diceValues);
  }

  isMe(player: PlayerDto): boolean {
    return player.playerId === this.myPlayerId;
  }

  get myPlayer(): PlayerDto | undefined {
    return this.players.find(p => p.playerId === this.myPlayerId);
  }

  onSelect(category: ScoreCategory): void {
    const me = this.players.find(p => p.playerId === this.myPlayerId);
    if (this.canSelect && me && !this.isUsed(me, category)) {
      this.selectCategory.emit(category);
    }
  }

  upperTotal(player: PlayerDto): number {
    return this.upperCategories.reduce((sum, cat) => {
      const e = player.scoreEntries.find(e => e.category === cat);
      return sum + (e?.isUsed ? (e.score ?? 0) : 0);
    }, 0);
  }

  hasBonus(player: PlayerDto): boolean { return this.upperTotal(player) >= 63; }

  // -------------------------------------------------------------------------
  // Client-side score suggestion (mirrors server logic)
  // -------------------------------------------------------------------------
  private calcScore(cat: ScoreCategory, d: number[]): number {
    const count = (v: number) => d.filter(x => x === v).length;
    const sorted = [...d].sort();
    const counts = Object.values(
      d.reduce((a: Record<number,number>, v) => { a[v] = (a[v]??0)+1; return a; }, {})
    ).sort((a,b)=>b-a);

    switch (cat) {
      case ScoreCategory.Ones:   return count(1)*1;
      case ScoreCategory.Twos:   return count(2)*2;
      case ScoreCategory.Threes: return count(3)*3;
      case ScoreCategory.Fours:  return count(4)*4;
      case ScoreCategory.Fives:  return count(5)*5;
      case ScoreCategory.Sixes:  return count(6)*6;
      case ScoreCategory.OnePair: {
        for (let v=6;v>=1;v--) if (count(v)>=2) return v*2; return 0;
      }
      case ScoreCategory.TwoPairs: {
        const pairs=[];for(let v=6;v>=1;v--)if(count(v)>=2)pairs.push(v);
        return pairs.length>=2?pairs[0]*2+pairs[1]*2:0;
      }
      case ScoreCategory.ThreeOfAKind: {
        for(let v=6;v>=1;v--)if(count(v)>=3)return v*3; return 0;
      }
      case ScoreCategory.FourOfAKind: {
        for(let v=6;v>=1;v--)if(count(v)>=4)return v*4; return 0;
      }
      case ScoreCategory.SmallStraight:
        return JSON.stringify(sorted)===JSON.stringify([1,2,3,4,5])?15:0;
      case ScoreCategory.LargeStraight:
        return JSON.stringify(sorted)===JSON.stringify([2,3,4,5,6])?20:0;
      case ScoreCategory.FullHouse:
        return counts[0]===3&&counts[1]===2?d.reduce((a,v)=>a+v,0):0;
      case ScoreCategory.Chance:
        return d.reduce((a,v)=>a+v,0);
      case ScoreCategory.Yatzy:
        return counts[0]===5?50:0;
      default: return 0;
    }
  }
}
