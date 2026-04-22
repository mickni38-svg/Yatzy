using Yatzy.Domain.Enums;

namespace Yatzy.Domain.Entities;

public sealed class Player
{
    public Guid Id { get; private set; }
    public string DisplayName { get; private set; } = string.Empty;
    public bool IsConnected { get; private set; }
    public bool HasLeft { get; private set; }
    public ScoreSheet ScoreSheet { get; private set; } = new();

    private Player() { }

    public static Player Create(Guid id, string displayName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

        return new Player
        {
            Id = id,
            DisplayName = displayName,
            IsConnected = true
        };
    }

    internal void SetConnected(bool connected) => IsConnected = connected;
    internal void Leave() { HasLeft = true; IsConnected = false; }

    // -------------------------------------------------------------------------
    // Persistence restore
    // -------------------------------------------------------------------------

    public static Player Restore(Guid id, string displayName, bool isConnected, bool hasLeft, Dictionary<ScoreCategory, int> scoreEntries)
    {
        var player = new Player
        {
            Id = id,
            DisplayName = displayName,
            IsConnected = isConnected,
            HasLeft = hasLeft
        };

        foreach (var (category, points) in scoreEntries)
            player.ScoreSheet.RegisterScore(category, points);

        return player;
    }
}
