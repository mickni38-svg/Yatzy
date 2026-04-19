using Yatzy.Application.DTOs;

namespace Yatzy.Application.Interfaces;

public interface IGameAppService
{
    Task<GameStateResponse> CreateGameAsync(CreateGameRequest request, CancellationToken cancellationToken = default);
    Task<GameStateResponse> JoinGameAsync(string roomCode, JoinGameRequest request, CancellationToken cancellationToken = default);
    Task<GameStateResponse> StartGameAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<GameStateResponse> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default);
    Task<GameStateResponse> GetByRoomCodeAsync(string roomCode, CancellationToken cancellationToken = default);
}
