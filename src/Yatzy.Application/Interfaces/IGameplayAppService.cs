using Yatzy.Application.DTOs;

namespace Yatzy.Application.Interfaces;

public interface IGameplayAppService
{
    Task<GameStateResponse> RollDiceAsync(RollDiceRequest request, CancellationToken cancellationToken = default);
    Task<GameStateResponse> ToggleHoldAsync(ToggleHoldRequest request, CancellationToken cancellationToken = default);
    Task<GameStateResponse> SelectScoreAsync(SelectScoreRequest request, CancellationToken cancellationToken = default);
    Task<GameStateResponse> PlayerDisconnectedAsync(Guid gameId, Guid playerId, CancellationToken cancellationToken = default);
    Task<GameStateResponse?> PlayerReconnectedAsync(Guid gameId, Guid playerId, CancellationToken cancellationToken = default);
}
