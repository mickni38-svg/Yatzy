using Yatzy.Domain.Enums;

namespace Yatzy.Domain.Interfaces;

public interface IScoreCalculator
{
    int Calculate(ScoreCategory category, IReadOnlyList<int> dice);
}
