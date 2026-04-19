namespace Yatzy.Application.DTOs;

public sealed class GameStateResponse
{
    public Guid GameId { get; init; }
    public string RoomCode { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int RoundNumber { get; init; }
    public int RollNumber { get; init; }
    public Guid? CurrentPlayerId { get; init; }
    public IReadOnlyList<PlayerDto> Players { get; init; } = [];
    public IReadOnlyList<DiceDto> Dice { get; init; } = [];
}

public sealed class PlayerDto
{
    public Guid PlayerId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public bool IsHost { get; init; }
    public bool IsConnected { get; init; }
    public int TotalScore { get; init; }
    public IReadOnlyList<ScoreEntryDto> ScoreEntries { get; init; } = [];
}

public sealed class DiceDto
{
    public int Position { get; init; }
    public int Value { get; init; }
    public bool IsHeld { get; init; }
}

public sealed class ScoreEntryDto
{
    public string Category { get; init; } = string.Empty;
    public int? Score { get; init; }
    public bool IsUsed { get; init; }
}
