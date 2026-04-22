namespace Yatzy.Persistence.Entities;

public sealed class PlayerEntity
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public bool HasLeft { get; set; }
    public int JoinOrder { get; set; }

    public GameEntity Game { get; set; } = null!;
    public List<ScoreEntryEntity> ScoreEntries { get; set; } = [];
}
