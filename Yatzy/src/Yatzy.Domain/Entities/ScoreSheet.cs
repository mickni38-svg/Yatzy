using Yatzy.Domain.Enums;

namespace Yatzy.Domain.Entities;

public sealed class ScoreSheet
{
    private static readonly ScoreCategory[] UpperCategories =
    [
        ScoreCategory.Ones, ScoreCategory.Twos, ScoreCategory.Threes,
        ScoreCategory.Fours, ScoreCategory.Fives, ScoreCategory.Sixes
    ];

    private readonly Dictionary<ScoreCategory, ScoreEntry> _entries;

    public IReadOnlyDictionary<ScoreCategory, ScoreEntry> Entries => _entries;

    public const int BonusThreshold = 63;
    public const int BonusValue = 50;

    public ScoreSheet()
    {
        _entries = Enum.GetValues<ScoreCategory>()
            .ToDictionary(c => c, c => new ScoreEntry(c));
    }

    public int UpperSectionSum =>
        UpperCategories
            .Where(c => _entries[c].IsUsed)
            .Sum(c => _entries[c].Score!.Value);

    public bool HasBonus => UpperSectionSum >= BonusThreshold;

    public int BonusScore => HasBonus ? BonusValue : 0;

    public int LowerSectionSum =>
        _entries.Values
            .Where(e => !UpperCategories.Contains(e.Category) && e.IsUsed)
            .Sum(e => e.Score!.Value);

    public int Total => UpperSectionSum + BonusScore + LowerSectionSum;

    public bool IsComplete => _entries.Values.All(e => e.IsUsed);

    public bool IsCategoryAvailable(ScoreCategory category) =>
        !_entries[category].IsUsed;

    internal void RegisterScore(ScoreCategory category, int score) =>
        _entries[category].Register(score);
}
