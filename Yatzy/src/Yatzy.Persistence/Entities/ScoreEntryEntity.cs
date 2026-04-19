using Yatzy.Domain.Enums;

namespace Yatzy.Persistence.Entities;

public sealed class ScoreEntryEntity
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public ScoreCategory Category { get; set; }
    public int Points { get; set; }

    public PlayerEntity Player { get; set; } = null!;
}
