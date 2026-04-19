using Yatzy.Domain.Interfaces;

namespace Yatzy.Infrastructure;

public sealed class RandomProvider : IRandomProvider
{
    public int Next(int minValue, int maxValue) =>
        Random.Shared.Next(minValue, maxValue);
}
