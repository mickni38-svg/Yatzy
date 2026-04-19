import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { CreateGameRequest, GameStateDto, JoinGameRequest } from '../models/game-state.dto';

@Injectable({ providedIn: 'root' })
export class GameApiService {
  private readonly base = environment.apiBaseUrl + '/games';

  constructor(private http: HttpClient) {}

  createGame(request: CreateGameRequest): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(this.base, request);
  }

  joinGame(roomCode: string, request: JoinGameRequest): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(`${this.base}/${roomCode}/join`, request);
  }

  startGame(gameId: string): Observable<GameStateDto> {
    return this.http.post<GameStateDto>(`${this.base}/${gameId}/start`, {});
  }

  getById(gameId: string): Observable<GameStateDto> {
    return this.http.get<GameStateDto>(`${this.base}/${gameId}`);
  }

  getByRoomCode(roomCode: string): Observable<GameStateDto> {
    return this.http.get<GameStateDto>(`${this.base}/by-room/${roomCode}`);
  }
}
