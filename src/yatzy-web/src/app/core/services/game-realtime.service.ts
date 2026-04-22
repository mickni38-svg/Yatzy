import { Injectable, OnDestroy } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { GameStateDto } from '../models/game-state.dto';
import { ScoreCategory } from '../../shared/enums/score-category.enum';

@Injectable({ providedIn: 'root' })
export class GameRealtimeService implements OnDestroy {
  private connection: signalR.HubConnection;

  private _gameState$ = new BehaviorSubject<GameStateDto | null>(null);
  private _error$ = new Subject<string>();
  private _connected$ = new BehaviorSubject<boolean>(false);

  readonly gameState$ = this._gameState$.asObservable();
  readonly error$ = this._error$.asObservable();
  readonly connected$ = this._connected$.asObservable();

  private currentRoomCode: string | null = null;
  private currentPlayerId: string | null = null;

  constructor() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(environment.hubUrl, {
        withCredentials: true
      })
      .withAutomaticReconnect()
      .configureLogging(signalR.LogLevel.Warning)
      .build();

    this.registerHandlers();

    this.connection.onreconnecting(() => this._connected$.next(false));
    this.connection.onreconnected(async () => {
      this._connected$.next(true);
      if (this.currentRoomCode && this.currentPlayerId) {
        await this.joinRoom(this.currentRoomCode, this.currentPlayerId);
      }
    });
    this.connection.onclose(() => this._connected$.next(false));
  }

  async start(): Promise<void> {
    if (this.connection.state === signalR.HubConnectionState.Disconnected) {
      await this.connection.start();
      this._connected$.next(true);
    }
  }

  async stop(): Promise<void> {
    await this.connection.stop();
    this._connected$.next(false);
  }

  async joinRoom(roomCode: string, playerId: string): Promise<void> {
    this.currentRoomCode = roomCode;
    this.currentPlayerId = playerId;
    await this.connection.invoke('JoinRoom', roomCode, playerId);
  }

  async startGame(gameId: string, playerId: string): Promise<void> {
    await this.connection.invoke('StartGame', { gameId, playerId });
  }

  async rollDice(gameId: string, playerId: string): Promise<void> {
    await this.connection.invoke('RollDice', { gameId, playerId });
  }

  async toggleHold(gameId: string, playerId: string, diceIndex: number): Promise<void> {
    await this.connection.invoke('ToggleHold', { gameId, playerId, diceIndex });
  }

  async selectScore(gameId: string, playerId: string, category: ScoreCategory): Promise<void> {
    await this.connection.invoke('SelectScore', { gameId, playerId, category });
  }

  async leaveGame(gameId: string, playerId: string): Promise<void> {
    await this.connection.invoke('LeaveGame', { gameId, playerId });
  }

  get currentState(): GameStateDto | null {
    return this._gameState$.value;
  }

  private registerHandlers(): void {
    const updateState = (state: GameStateDto) => this._gameState$.next(state);

    this.connection.on('GameStateUpdated', updateState);
    this.connection.on('PlayerJoined',     updateState);
    this.connection.on('PlayerLeft',       updateState);
    this.connection.on('GameStarted',      updateState);
    this.connection.on('DiceRolled',       updateState);
    this.connection.on('HoldChanged',      updateState);
    this.connection.on('ScoreSelected',    updateState);
    this.connection.on('GameEnded',        updateState);
    this.connection.on('Error', (msg: string) => this._error$.next(msg));
  }

  ngOnDestroy(): void {
    this.connection.stop();
  }
}
