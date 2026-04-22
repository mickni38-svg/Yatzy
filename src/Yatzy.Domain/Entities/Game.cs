using Die = Yatzy.Domain.Entities.Dice;
using Yatzy.Domain.Enums;
using Yatzy.Domain.Exceptions;
using Yatzy.Domain.Interfaces;

namespace Yatzy.Domain.Entities;

public sealed class Game
{
    public const int MaxPlayers = 6;
    public const int MinPlayersToStart = 2;
    public const int DiceCount = 5;
    public const int MaxRollsPerTurn = 3;

    public Guid Id { get; private set; }
    public string RoomCode { get; private set; } = string.Empty;
    public GameStatus Status { get; private set; }
    public int CurrentPlayerIndex { get; private set; }
    public int RoundNumber { get; private set; }
    public int RollNumber { get; private set; }

    private readonly List<Player> _players = [];
    private readonly List<Dice> _dice = [];

    public IReadOnlyList<Player> Players => _players;
    public IReadOnlyList<Dice> Dice => _dice;

    public Player? CurrentPlayer =>
        _players.Count > 0 ? _players[CurrentPlayerIndex] : null;

    private Game() { }

    public static Game Create(string roomCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(roomCode);

        var game = new Game
        {
            Id = Guid.NewGuid(),
            RoomCode = roomCode.ToUpperInvariant(),
            Status = GameStatus.WaitingForPlayers
        };

        for (int i = 0; i < DiceCount; i++)
            game._dice.Add(Die.Create(i));

        return game;
    }

    // -------------------------------------------------------------------------
    // Lobby management
    // -------------------------------------------------------------------------

    public Player AddPlayer(Guid playerId, string displayName)
    {
        if (Status != GameStatus.WaitingForPlayers && Status != GameStatus.ReadyToStart)
            throw new DomainException("Cannot join a game that is already in progress or completed.");

        if (_players.Count >= MaxPlayers)
            throw new DomainException($"Game is full. Maximum {MaxPlayers} players allowed.");

        if (_players.Any(p => p.Id == playerId))
            throw new DomainException("Player is already in the game.");

        var player = Player.Create(playerId, displayName);
        _players.Add(player);

        if (_players.Count >= MinPlayersToStart)
            Status = GameStatus.ReadyToStart;

        return player;
    }

    public void SetPlayerConnected(Guid playerId, bool connected)
    {
        var player = RequirePlayer(playerId);
        player.SetConnected(connected);
    }

    public void LeaveGame(Guid playerId)
    {
        RequireInProgress();
        var player = RequirePlayer(playerId);
        player.Leave();

        // If it was this player's turn, advance immediately
        if (CurrentPlayer?.Id == playerId)
            AdvanceTurn();
    }

    // -------------------------------------------------------------------------
    // Game flow
    // -------------------------------------------------------------------------

    public void StartGame()
    {
        if (Status != GameStatus.ReadyToStart)
            throw new DomainException("Game cannot be started in its current state.");

        Status = GameStatus.InProgress;
        CurrentPlayerIndex = 0;
        RoundNumber = 1;
        RollNumber = 0;

        ResetDice();
    }

    public void RollDice(Guid playerId, IRandomProvider random)
    {
        RequireInProgress();
        RequireCurrentPlayer(playerId);

        if (RollNumber >= MaxRollsPerTurn)
            throw new DomainException("Cannot roll more than 3 times per turn.");

        RollNumber++;

        foreach (var die in _dice)
            die.Roll(random.Next(1, 7));
    }

    public void ToggleHold(Guid playerId, int diceIndex)
    {
        RequireInProgress();
        RequireCurrentPlayer(playerId);

        if (RollNumber == 0)
            throw new DomainException("Cannot hold dice before rolling.");

        if (diceIndex < 0 || diceIndex >= DiceCount)
            throw new DomainException($"Dice index must be between 0 and {DiceCount - 1}.");

        _dice[diceIndex].ToggleHold();
    }

    public void SelectScore(Guid playerId, ScoreCategory category, IScoreCalculator calculator)
    {
        RequireInProgress();
        RequireCurrentPlayer(playerId);

        if (RollNumber == 0)
            throw new DomainException("Cannot select a score category before rolling.");

        var player = RequirePlayer(playerId);

        if (!player.ScoreSheet.IsCategoryAvailable(category))
            throw new DomainException($"Category {category} has already been used.");

        var diceValues = _dice.Select(d => d.Value).ToList();
        int score = calculator.Calculate(category, diceValues);

        player.ScoreSheet.RegisterScore(category, score);

        AdvanceTurn();
    }

    // -------------------------------------------------------------------------
    // Queries
    // -------------------------------------------------------------------------

    public bool CanRoll(Guid playerId) =>
        Status == GameStatus.InProgress &&
        CurrentPlayer?.Id == playerId &&
        RollNumber < MaxRollsPerTurn;

    public bool CanSelectCategory(Guid playerId, ScoreCategory category) =>
        Status == GameStatus.InProgress &&
        CurrentPlayer?.Id == playerId &&
        RollNumber > 0 &&
        (RequirePlayer(playerId).ScoreSheet.IsCategoryAvailable(category));

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private void AdvanceTurn()
    {
        ResetDice();

        // If all active (non-left) players have completed their scoresheet → game over
        if (_players.All(p => p.HasLeft || p.ScoreSheet.IsComplete))
        {
            Status = GameStatus.Completed;
            return;
        }

        // Advance to next player who hasn't left and still has categories left
        int startIndex = CurrentPlayerIndex;
        do
        {
            CurrentPlayerIndex = (CurrentPlayerIndex + 1) % _players.Count;
            if (CurrentPlayerIndex == 0)
                RoundNumber++;
        }
        while ((_players[CurrentPlayerIndex].HasLeft || _players[CurrentPlayerIndex].ScoreSheet.IsComplete) &&
               CurrentPlayerIndex != startIndex);

        RollNumber = 0;
    }

    private void ResetDice()
    {
        foreach (var die in _dice)
            die.ReleaseHold();
    }

    private void RequireInProgress()
    {
        if (Status != GameStatus.InProgress)
            throw new DomainException("Game is not in progress.");
    }

    private void RequireCurrentPlayer(Guid playerId)
    {
        if (CurrentPlayer?.Id != playerId)
            throw new DomainException("It is not this player's turn.");
    }

    private Player RequirePlayer(Guid playerId) =>
        _players.FirstOrDefault(p => p.Id == playerId)
        ?? throw new DomainException($"Player {playerId} is not in this game.");

    // -------------------------------------------------------------------------
    // Persistence restore
    // -------------------------------------------------------------------------

    public static Game Restore(
        Guid id,
        string roomCode,
        GameStatus status,
        int currentPlayerIndex,
        int roundNumber,
        int rollNumber,
        IEnumerable<Player> players,
        IEnumerable<Dice> dice)
    {
        var game = new Game
        {
            Id = id,
            RoomCode = roomCode,
            Status = status,
            CurrentPlayerIndex = currentPlayerIndex,
            RoundNumber = roundNumber,
            RollNumber = rollNumber
        };

        game._players.AddRange(players);
        game._dice.AddRange(dice);

        return game;
    }
}
