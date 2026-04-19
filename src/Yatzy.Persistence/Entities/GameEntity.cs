using Yatzy.Domain.Enums;

namespace Yatzy.Persistence.Entities;

public sealed class GameEntity
{
    public Guid Id { get; set; }
    public string RoomCode { get; set; } = string.Empty;
    public GameStatus Status { get; set; }
    public int CurrentPlayerIndex { get; set; }
    public int RoundNumber { get; set; }
    public int RollNumber { get; set; }
    public DateTime CreatedUtc { get; set; }
    public DateTime UpdatedUtc { get; set; }

    public List<PlayerEntity> Players { get; set; } = [];
    public List<DiceEntity> Dice { get; set; } = [];
}
