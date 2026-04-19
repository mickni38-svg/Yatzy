using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yatzy.Domain.Enums;
using Yatzy.Domain.Rules;

namespace Yatzy.Domain.Tests.Scoring;

[TestClass]
public sealed class ScoreCalculatorTests
{
    private readonly ScoreCalculator _sut = new();

    // Ones
    [TestMethod] public void Ones_ReturnsSumOfOnes()
        => Assert.AreEqual(3, _sut.Calculate(ScoreCategory.Ones, new[]{1,1,1,2,3}));
    [TestMethod] public void Ones_NoOnes_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.Ones, new[]{2,3,4,5,6}));

    // Twos
    [TestMethod] public void Twos_ReturnsSumOfTwos()
        => Assert.AreEqual(6, _sut.Calculate(ScoreCategory.Twos, new[]{2,2,2,1,3}));

    // Threes
    [TestMethod] public void Threes_ReturnsSumOfThrees()
        => Assert.AreEqual(9, _sut.Calculate(ScoreCategory.Threes, new[]{3,3,3,1,2}));

    // Fours
    [TestMethod] public void Fours_ReturnsSumOfFours()
        => Assert.AreEqual(8, _sut.Calculate(ScoreCategory.Fours, new[]{4,4,1,2,3}));

    // Fives
    [TestMethod] public void Fives_ReturnsSumOfFives()
        => Assert.AreEqual(15, _sut.Calculate(ScoreCategory.Fives, new[]{5,5,5,1,2}));

    // Sixes
    [TestMethod] public void Sixes_ReturnsSumOfSixes()
        => Assert.AreEqual(12, _sut.Calculate(ScoreCategory.Sixes, new[]{6,6,1,2,3}));

    // One Pair
    [TestMethod] public void OnePair_ReturnsBestPairSum()
        => Assert.AreEqual(10, _sut.Calculate(ScoreCategory.OnePair, new[]{5,5,1,2,3}));
    [TestMethod] public void OnePair_MultiplePairs_ReturnsBest()
        => Assert.AreEqual(12, _sut.Calculate(ScoreCategory.OnePair, new[]{6,6,5,5,1}));
    [TestMethod] public void OnePair_NoPair_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.OnePair, new[]{1,2,3,4,5}));

    // Two Pairs
    [TestMethod] public void TwoPairs_ReturnsSumOfBothPairs()
        => Assert.AreEqual(16, _sut.Calculate(ScoreCategory.TwoPairs, new[]{3,3,5,5,1}));
    [TestMethod] public void TwoPairs_OnlyOnePair_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.TwoPairs, new[]{3,3,1,2,4}));
    [TestMethod] public void TwoPairs_FullHouse_CountsAsTwoPairs()
        => Assert.AreEqual(10, _sut.Calculate(ScoreCategory.TwoPairs, new[]{2,2,3,3,3}));

    // Three of a Kind
    [TestMethod] public void ThreeOfAKind_ReturnsThreeTimes()
        => Assert.AreEqual(9, _sut.Calculate(ScoreCategory.ThreeOfAKind, new[]{3,3,3,1,2}));
    [TestMethod] public void ThreeOfAKind_NoThree_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.ThreeOfAKind, new[]{1,2,3,4,5}));

    // Four of a Kind
    [TestMethod] public void FourOfAKind_ReturnsFourTimes()
        => Assert.AreEqual(20, _sut.Calculate(ScoreCategory.FourOfAKind, new[]{5,5,5,5,1}));
    [TestMethod] public void FourOfAKind_NoFour_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.FourOfAKind, new[]{5,5,5,1,2}));

    // Small Straight
    [TestMethod] public void SmallStraight_Returns15()
        => Assert.AreEqual(15, _sut.Calculate(ScoreCategory.SmallStraight, new[]{1,2,3,4,5}));
    [TestMethod] public void SmallStraight_WrongDice_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.SmallStraight, new[]{2,3,4,5,6}));

    // Large Straight
    [TestMethod] public void LargeStraight_Returns20()
        => Assert.AreEqual(20, _sut.Calculate(ScoreCategory.LargeStraight, new[]{2,3,4,5,6}));
    [TestMethod] public void LargeStraight_WrongDice_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.LargeStraight, new[]{1,2,3,4,5}));

    // Full House
    [TestMethod] public void FullHouse_ReturnsSumWhenValid()
        => Assert.AreEqual(13, _sut.Calculate(ScoreCategory.FullHouse, new[]{2,2,3,3,3}));
    [TestMethod] public void FullHouse_FourOfAKind_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.FullHouse, new[]{1,1,1,1,2}));

    // Chance
    [TestMethod] public void Chance_ReturnsSumOfAllDice()
        => Assert.AreEqual(15, _sut.Calculate(ScoreCategory.Chance, new[]{1,2,3,4,5}));

    // Yatzy
    [TestMethod] public void Yatzy_Returns50WhenAllSame()
        => Assert.AreEqual(50, _sut.Calculate(ScoreCategory.Yatzy, new[]{6,6,6,6,6}));
    [TestMethod] public void Yatzy_NotAllSame_ReturnsZero()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.Yatzy, new[]{1,1,1,1,2}));

    // Bonus threshold edge cases (tested via ScoreSheet directly)
    [TestMethod] public void Chance_AllSixes_Returns30()
        => Assert.AreEqual(30, _sut.Calculate(ScoreCategory.Chance, new[]{6,6,6,6,6}));

    [TestMethod] public void FullHouse_TwoPairVariant_ReturnsZeroForFullHouse()
        => Assert.AreEqual(0, _sut.Calculate(ScoreCategory.FullHouse, new[]{1,1,2,2,3}));

    // Guard clauses
    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Calculate_ThrowsWhenNotFiveDice()
        => _sut.Calculate(ScoreCategory.Chance, new[]{1,2,3});

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Calculate_ThrowsWhenDiceValueOutOfRange()
        => _sut.Calculate(ScoreCategory.Chance, new[]{1,2,3,4,7});
}
