using Yatzy.Application.DTOs;

namespace Yatzy.Application.Interfaces;

public interface IGameHubService
{
    Task BroadcastGameStateAsync(string roomCode, GameStateResponse state);
    Task BroadcastPlayerJoinedAsync(string roomCode, GameStateResponse state);
    Task BroadcastPlayerLeftAsync(string roomCode, GameStateResponse state);
    Task BroadcastGameStartedAsync(string roomCode, GameStateResponse state);
    Task BroadcastDiceRolledAsync(string roomCode, GameStateResponse state);
    Task BroadcastHoldChangedAsync(string roomCode, GameStateResponse state);
    Task BroadcastScoreSelectedAsync(string roomCode, GameStateResponse state);
    Task BroadcastGameEndedAsync(string roomCode, GameStateResponse state);
}
