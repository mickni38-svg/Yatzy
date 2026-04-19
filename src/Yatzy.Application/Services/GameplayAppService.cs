using Yatzy.Application.DTOs;
using Yatzy.Application.Interfaces;
using Yatzy.Domain.Enums;
using Yatzy.Domain.Exceptions;
using Yatzy.Domain.Interfaces;

namespace Yatzy.Application.Services;

public sealed class GameplayAppService : IGameplayAppService
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRandomProvider _random;
    private readonly IScoreCalculator _scoreCalculator;

    public GameplayAppService(
        IGameRepository gameRepository,
        IUnitOfWork unitOfWork,
        IRandomProvider random,
        IScoreCalculator scoreCalculator)
    {
        _gameRepository = gameRepository;
        _unitOfWork = unitOfWork;
        _random = random;
        _scoreCalculator = scoreCalculator;
    }

    public async Task<GameStateResponse> RollDiceAsync(RollDiceRequest request, CancellationToken cancellationToken = default)
    {
        var game = await RequireGameAsync(request.GameId, cancellationToken);

        game.RollDice(request.PlayerId, _random);

        _gameRepository.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameStateResponse> ToggleHoldAsync(ToggleHoldRequest request, CancellationToken cancellationToken = default)
    {
        var game = await RequireGameAsync(request.GameId, cancellationToken);

        game.ToggleHold(request.PlayerId, request.DiceIndex);

        _gameRepository.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameStateResponse> SelectScoreAsync(SelectScoreRequest request, CancellationToken cancellationToken = default)
    {
        var game = await RequireGameAsync(request.GameId, cancellationToken);

        game.SelectScore(request.PlayerId, request.Category, _scoreCalculator);

        _gameRepository.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameStateResponse> PlayerDisconnectedAsync(Guid gameId, Guid playerId, CancellationToken cancellationToken = default)
    {
        var game = await RequireGameAsync(gameId, cancellationToken);

        game.SetPlayerConnected(playerId, false);

        _gameRepository.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameStateResponse?> PlayerReconnectedAsync(Guid gameId, Guid playerId, CancellationToken cancellationToken = default)
    {
        var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
        if (game is null)
            return null;

        game.SetPlayerConnected(playerId, true);

        _gameRepository.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<Domain.Entities.Game> RequireGameAsync(Guid gameId, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
        if (game is null)
            throw new NotFoundException($"Game {gameId} was not found.");
        return game;
    }

    // -------------------------------------------------------------------------
    // Mapping — reuses same shape as GameAppService
    // -------------------------------------------------------------------------

    private static GameStateResponse MapToResponse(Domain.Entities.Game game) => new()
    {
        GameId = game.Id,
        RoomCode = game.RoomCode,
        Status = game.Status.ToString(),
        RoundNumber = game.RoundNumber,
        RollNumber = game.RollNumber,
        CurrentPlayerId = game.CurrentPlayer?.Id,
        Players = game.Players
            .Select((p, i) => new PlayerDto
            {
                PlayerId = p.Id,
                DisplayName = p.DisplayName,
                IsHost = i == 0,
                IsConnected = p.IsConnected,
                TotalScore = p.ScoreSheet.Total,
                ScoreEntries = p.ScoreSheet.Entries
                    .Select(kvp => new ScoreEntryDto
                    {
                        Category = kvp.Key.ToString(),
                        Score = kvp.Value.Score,
                        IsUsed = kvp.Value.IsUsed
                    })
                    .ToList()
            })
            .ToList(),
        Dice = game.Dice
            .Select(d => new DiceDto
            {
                Position = d.Position,
                Value = d.Value,
                IsHeld = d.IsHeld
            })
            .ToList()
    };
}
