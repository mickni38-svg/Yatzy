namespace Yatzy.Domain.Interfaces;

public interface IRandomProvider
{
    int Next(int minValue, int maxValue);
}
