namespace Yatzy.Domain.Entities;

public sealed class Dice
{
    public int Position { get; private set; }
    public int Value { get; private set; }
    public bool IsHeld { get; private set; }

    private Dice() { }

    public static Dice Create(int position)
    {
        if (position < 0 || position > 4)
            throw new ArgumentOutOfRangeException(nameof(position), "Dice position must be between 0 and 4.");

        return new Dice { Position = position, Value = 1 };
    }

    internal void Roll(int value)
    {
        if (value < 1 || value > 6)
            throw new ArgumentOutOfRangeException(nameof(value), "Dice value must be between 1 and 6.");

        if (!IsHeld)
            Value = value;
    }

    internal void ToggleHold() => IsHeld = !IsHeld;

    internal void ReleaseHold() => IsHeld = false;

    public static Dice Restore(int position, int value, bool isHeld) =>
        new() { Position = position, Value = value, IsHeld = isHeld };
}
