namespace Yatzy.Persistence.Entities;

public sealed class DiceEntity
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public int Position { get; set; }
    public int Value { get; set; }
    public bool IsHeld { get; set; }

    public GameEntity Game { get; set; } = null!;
}
