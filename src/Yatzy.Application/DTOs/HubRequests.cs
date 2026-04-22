using Yatzy.Domain.Enums;

namespace Yatzy.Application.DTOs;

public sealed record RollDiceRequest(Guid GameId, Guid PlayerId);
public sealed record ToggleHoldRequest(Guid GameId, Guid PlayerId, int DiceIndex);
public sealed record SelectScoreRequest(Guid GameId, Guid PlayerId, ScoreCategory Category);
public sealed record StartGameRequest(Guid GameId, Guid PlayerId);
public sealed record LeaveGameRequest(Guid GameId, Guid PlayerId);
