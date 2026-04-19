using Yatzy.Domain.Enums;
using Yatzy.Domain.Interfaces;

namespace Yatzy.Domain.Rules;

public sealed class ScoreCalculator : IScoreCalculator
{
    public int Calculate(ScoreCategory category, IReadOnlyList<int> dice)
    {
        if (dice.Count != 5)
            throw new ArgumentException("Exactly 5 dice values are required.", nameof(dice));

        if (dice.Any(d => d < 1 || d > 6))
            throw new ArgumentException("Dice values must be between 1 and 6.", nameof(dice));

        return category switch
        {
            ScoreCategory.Ones         => SumOfFace(dice, 1),
            ScoreCategory.Twos         => SumOfFace(dice, 2),
            ScoreCategory.Threes       => SumOfFace(dice, 3),
            ScoreCategory.Fours        => SumOfFace(dice, 4),
            ScoreCategory.Fives        => SumOfFace(dice, 5),
            ScoreCategory.Sixes        => SumOfFace(dice, 6),
            ScoreCategory.OnePair      => BestPair(dice),
            ScoreCategory.TwoPairs     => TwoPairs(dice),
            ScoreCategory.ThreeOfAKind => ThreeOfAKind(dice),
            ScoreCategory.FourOfAKind  => FourOfAKind(dice),
            ScoreCategory.SmallStraight => SmallStraight(dice),
            ScoreCategory.LargeStraight => LargeStraight(dice),
            ScoreCategory.FullHouse    => FullHouse(dice),
            ScoreCategory.Chance       => dice.Sum(),
            ScoreCategory.Yatzy        => Yatzy(dice),
            _ => throw new ArgumentOutOfRangeException(nameof(category))
        };
    }

    // -------------------------------------------------------------------------
    // Upper section
    // -------------------------------------------------------------------------

    private static int SumOfFace(IReadOnlyList<int> dice, int face) =>
        dice.Where(d => d == face).Sum();

    // -------------------------------------------------------------------------
    // Lower section
    // -------------------------------------------------------------------------

    private static int BestPair(IReadOnlyList<int> dice)
    {
        var pair = Counts(dice)
            .Where(kv => kv.Value >= 2)
            .Select(kv => kv.Key)
            .OrderByDescending(k => k)
            .FirstOrDefault();

        return pair * 2;
    }

    private static int TwoPairs(IReadOnlyList<int> dice)
    {
        var pairs = Counts(dice)
            .Where(kv => kv.Value >= 2)
            .Select(kv => kv.Key)
            .OrderByDescending(k => k)
            .Take(2)
            .ToList();

        return pairs.Count == 2 ? pairs.Sum(p => p * 2) : 0;
    }

    private static int ThreeOfAKind(IReadOnlyList<int> dice)
    {
        var face = Counts(dice)
            .Where(kv => kv.Value >= 3)
            .Select(kv => kv.Key)
            .OrderByDescending(k => k)
            .FirstOrDefault();

        return face * 3;
    }

    private static int FourOfAKind(IReadOnlyList<int> dice)
    {
        var face = Counts(dice)
            .Where(kv => kv.Value >= 4)
            .Select(kv => kv.Key)
            .OrderByDescending(k => k)
            .FirstOrDefault();

        return face * 4;
    }

    private static int SmallStraight(IReadOnlyList<int> dice)
    {
        var sorted = dice.Distinct().OrderBy(d => d).ToList();
        return sorted.SequenceEqual([1, 2, 3, 4, 5]) ? 15 : 0;
    }

    private static int LargeStraight(IReadOnlyList<int> dice)
    {
        var sorted = dice.Distinct().OrderBy(d => d).ToList();
        return sorted.SequenceEqual([2, 3, 4, 5, 6]) ? 20 : 0;
    }

    private static int FullHouse(IReadOnlyList<int> dice)
    {
        var counts = Counts(dice);
        bool hasThree = counts.Any(kv => kv.Value == 3);
        bool hasTwo = counts.Any(kv => kv.Value == 2);

        return hasThree && hasTwo ? dice.Sum() : 0;
    }

    private static int Yatzy(IReadOnlyList<int> dice) =>
        dice.Distinct().Count() == 1 ? 50 : 0;

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Dictionary<int, int> Counts(IReadOnlyList<int> dice) =>
        dice.GroupBy(d => d).ToDictionary(g => g.Key, g => g.Count());
}
