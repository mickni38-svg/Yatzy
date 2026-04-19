using Yatzy.Domain.Enums;

namespace Yatzy.Domain.Entities;

public sealed class ScoreEntry
{
    public ScoreCategory Category { get; }
    public int? Score { get; private set; }
    public bool IsUsed => Score.HasValue;

    public ScoreEntry(ScoreCategory category)
    {
        Category = category;
    }

    internal void Register(int score)
    {
        if (IsUsed)
            throw new InvalidOperationException($"Category {Category} has already been used.");

        Score = score;
    }
}
