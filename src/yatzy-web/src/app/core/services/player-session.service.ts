import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class PlayerSessionService {
  private readonly PLAYER_ID_KEY = 'yatzy_player_id';
  private readonly GAME_ID_KEY   = 'yatzy_game_id';
  private readonly ROOM_CODE_KEY = 'yatzy_room_code';

  get playerId(): string | null {
    return sessionStorage.getItem(this.PLAYER_ID_KEY);
  }

  get gameId(): string | null {
    return sessionStorage.getItem(this.GAME_ID_KEY);
  }

  get roomCode(): string | null {
    return sessionStorage.getItem(this.ROOM_CODE_KEY);
  }

  save(playerId: string, gameId: string, roomCode: string): void {
    sessionStorage.setItem(this.PLAYER_ID_KEY, playerId);
    sessionStorage.setItem(this.GAME_ID_KEY, gameId);
    sessionStorage.setItem(this.ROOM_CODE_KEY, roomCode);
  }

  clear(): void {
    sessionStorage.removeItem(this.PLAYER_ID_KEY);
    sessionStorage.removeItem(this.GAME_ID_KEY);
    sessionStorage.removeItem(this.ROOM_CODE_KEY);
  }

  hasSession(): boolean {
    return !!this.playerId && !!this.gameId && !!this.roomCode;
  }
}
