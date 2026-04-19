using Microsoft.VisualStudio.TestTools.UnitTesting;
using Yatzy.Domain.Entities;
using Yatzy.Domain.Enums;
using Yatzy.Domain.Exceptions;
using Yatzy.Domain.Interfaces;
using Yatzy.Domain.Rules;

namespace Yatzy.Domain.Tests.GameTests;

[TestClass]
public sealed class GameTurnTests
{
    private readonly IScoreCalculator _calc = new ScoreCalculator();

    private static (Game game, Guid p1, Guid p2) StartedGame()
    {
        var game = Game.Create("ABCD");
        var p1 = Guid.NewGuid(); game.AddPlayer(p1, "Alice");
        var p2 = Guid.NewGuid(); game.AddPlayer(p2, "Bob");
        game.StartGame();
        return (game, p1, p2);
    }

    // --- Lobby ---

    [TestMethod]
    public void Create_ValidRoomCode_SetsStatusWaitingForPlayers()
    {
        var game = Game.Create("xyz");
        Assert.AreEqual(GameStatus.WaitingForPlayers, game.Status);
        Assert.AreEqual("XYZ", game.RoomCode);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public void Create_EmptyRoomCode_Throws()
        => Game.Create("   ");

    [TestMethod]
    public void AddPlayer_UnderLimit_AddsSuccessfully()
    {
        var game = Game.Create("X1");
        game.AddPlayer(Guid.NewGuid(), "Alice");
        Assert.AreEqual(1, game.Players.Count);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void AddPlayer_AtMaxCapacity_Throws()
    {
        var game = Game.Create("X1");
        for (int i = 0; i < Game.MaxPlayers + 1; i++)
            game.AddPlayer(Guid.NewGuid(), "P" + i);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void StartGame_OnlyOnePlayer_Throws()
    {
        var game = Game.Create("X1");
        game.AddPlayer(Guid.NewGuid(), "Alice");
        game.StartGame();
    }

    [TestMethod]
    public void StartGame_TwoPlayers_SetsStatusInProgress()
    {
        var (game, _, _) = StartedGame();
        Assert.AreEqual(GameStatus.InProgress, game.Status);
    }

    // --- Rolling ---

    [TestMethod]
    public void RollDice_FirstRoll_AllDiceGetValues()
    {
        var (game, p1, _) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(3));
        Assert.IsTrue(game.Dice.All(d => d.Value == 3));
    }

    [TestMethod]
    public void RollDice_HeldDiceAreNotRerolled()
    {
        var (game, p1, _) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(4));
        game.ToggleHold(p1, 0);
        game.RollDice(p1, new FixedRandomProvider(6));
        Assert.AreEqual(4, game.Dice[0].Value);
        Assert.IsTrue(game.Dice.Skip(1).All(d => d.Value == 6));
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void RollDice_AfterThreeRolls_Throws()
    {
        var (game, p1, _) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(1));
        game.RollDice(p1, new FixedRandomProvider(2));
        game.RollDice(p1, new FixedRandomProvider(3));
        game.RollDice(p1, new FixedRandomProvider(4));
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void RollDice_WrongPlayer_Throws()
    {
        var (game, _, p2) = StartedGame();
        game.RollDice(p2, new FixedRandomProvider(1));
    }

    // --- Scoring ---

    [TestMethod]
    public void SelectScore_ValidCategory_RegistersScore()
    {
        var (game, p1, _) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(1));
        game.SelectScore(p1, ScoreCategory.Ones, _calc);
        Assert.IsFalse(game.Players[0].ScoreSheet.IsCategoryAvailable(ScoreCategory.Ones));
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void SelectScore_WithoutRollingFirst_Throws()
    {
        var (game, p1, _) = StartedGame();
        game.SelectScore(p1, ScoreCategory.Chance, _calc);
    }

    [TestMethod]
    public void SelectScore_AdvancesToNextPlayer()
    {
        var (game, p1, _) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(1));
        game.SelectScore(p1, ScoreCategory.Ones, _calc);
        Assert.AreEqual(1, game.CurrentPlayerIndex);
    }

    [TestMethod]
    public void SelectScore_LastPlayerScores_WrapsBackToFirstPlayer()
    {
        var (game, p1, p2) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(1));
        game.SelectScore(p1, ScoreCategory.Ones, _calc);
        game.RollDice(p2, new FixedRandomProvider(2));
        game.SelectScore(p2, ScoreCategory.Twos, _calc);
        Assert.AreEqual(0, game.CurrentPlayerIndex);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void SelectScore_UsedCategory_Throws()
    {
        var (game, p1, p2) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(1));
        game.SelectScore(p1, ScoreCategory.Ones, _calc);
        game.RollDice(p2, new FixedRandomProvider(2));
        game.SelectScore(p2, ScoreCategory.Twos, _calc);
        // p1 tries to use Ones again
        game.RollDice(p1, new FixedRandomProvider(1));
        game.SelectScore(p1, ScoreCategory.Ones, _calc);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void ToggleHold_WrongPlayer_Throws()
    {
        var (game, p1, p2) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(3));
        game.ToggleHold(p2, 0); // p2 tries to hold during p1's turn
    }

    // --- Hold toggle ---

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void ToggleHold_BeforeFirstRoll_Throws()
    {
        var (game, p1, _) = StartedGame();
        game.ToggleHold(p1, 0);
    }

    [TestMethod]
    public void ToggleHold_AfterRoll_TogglesHoldState()
    {
        var (game, p1, _) = StartedGame();
        game.RollDice(p1, new FixedRandomProvider(5));
        game.ToggleHold(p1, 2);
        Assert.IsTrue(game.Dice[2].IsHeld);
        game.ToggleHold(p1, 2);
        Assert.IsFalse(game.Dice[2].IsHeld);
    }

    // --- Bonus ---

    [TestMethod]
    public void UpperSectionBonus_ScoreAboveThreshold_GivesBonus()
    {
        var (game, p1, p2) = StartedGame();
        var categories = new[] {
            ScoreCategory.Ones, ScoreCategory.Twos, ScoreCategory.Threes,
            ScoreCategory.Fours, ScoreCategory.Fives, ScoreCategory.Sixes
        };
        var faces = new[] { 1, 2, 3, 4, 5, 6 };
        for (int i = 0; i < categories.Length; i++)
        {
            game.RollDice(p1, new FixedRandomProvider(faces[i]));
            game.SelectScore(p1, categories[i], _calc);
            game.RollDice(p2, new FixedRandomProvider(faces[i]));
            game.SelectScore(p2, categories[i], _calc);
        }
        Assert.AreEqual(50, game.Players[0].ScoreSheet.BonusScore);
    }

    // --- Game completion ---

    [TestMethod]
    public void Game_AllCategoriesFilled_StatusBecomesCompleted()
    {
        var (game, p1, p2) = StartedGame();
        var categories = (ScoreCategory[])Enum.GetValues(typeof(ScoreCategory));
        foreach (var cat in categories)
        {
            game.RollDice(p1, new FixedRandomProvider(1));
            game.SelectScore(p1, cat, _calc);
            game.RollDice(p2, new FixedRandomProvider(1));
            game.SelectScore(p2, cat, _calc);
        }
        Assert.AreEqual(GameStatus.Completed, game.Status);
    }
}

internal sealed class FixedRandomProvider(int value) : IRandomProvider
{
    public int Next(int min, int max) => value;
}


