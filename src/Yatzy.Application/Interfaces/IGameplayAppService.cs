using Yatzy.Application.DTOs;

namespace Yatzy.Application.Interfaces;

public interface IGameplayAppService
{
    Task<GameStateResponse> RollDiceAsync(RollDiceRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Beregner terningresultatet og starter DB-skrivning — returnerer state med det samme
    /// så broadcast kan ske parallelt med gem-operationen.
    /// </summary>
    Task<(GameStateResponse State, Task PersistTask)> RollDiceFastAsync(RollDiceRequest request, CancellationToken cancellationToken = default);

    Task<GameStateResponse> ToggleHoldAsync(ToggleHoldRequest request, CancellationToken cancellationToken = default);
    Task<GameStateResponse> SelectScoreAsync(SelectScoreRequest request, CancellationToken cancellationToken = default);
    Task<GameStateResponse> PlayerDisconnectedAsync(Guid gameId, Guid playerId, CancellationToken cancellationToken = default);
    Task<GameStateResponse?> PlayerReconnectedAsync(Guid gameId, Guid playerId, CancellationToken cancellationToken = default);
    Task<GameStateResponse> LeaveGameAsync(LeaveGameRequest request, CancellationToken cancellationToken = default);
}
