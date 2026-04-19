export interface GameStateDto {
  gameId: string;
  roomCode: string;
  status: string;
  roundNumber: number;
  rollNumber: number;
  currentPlayerId: string | null;
  players: PlayerDto[];
  dice: DiceDto[];
}

export interface PlayerDto {
  playerId: string;
  displayName: string;
  isHost: boolean;
  isConnected: boolean;
  totalScore: number;
  scoreEntries: ScoreEntryDto[];
}

export interface DiceDto {
  position: number;
  value: number;
  isHeld: boolean;
}

export interface ScoreEntryDto {
  category: string;
  score: number | null;
  isUsed: boolean;
}

export interface CreateGameRequest {
  hostName: string;
}

export interface JoinGameRequest {
  playerName: string;
}
