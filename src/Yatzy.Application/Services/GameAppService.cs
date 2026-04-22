using Yatzy.Application.DTOs;
using Yatzy.Application.Interfaces;
using Yatzy.Domain.Entities;
using Yatzy.Domain.Exceptions;
using Yatzy.Domain.Interfaces;

namespace Yatzy.Application.Services;

public sealed class GameAppService : IGameAppService
{
    private readonly IGameRepository _gameRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRandomProvider _random;

    public GameAppService(
        IGameRepository gameRepository,
        IUnitOfWork unitOfWork,
        IRandomProvider random)
    {
        _gameRepository = gameRepository;
        _unitOfWork = unitOfWork;
        _random = random;
    }

    public async Task<GameStateResponse> CreateGameAsync(CreateGameRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.HostName))
            throw new ValidationException("HostName is required.");

        var roomCode = GenerateRoomCode();
        var game = Game.Create(roomCode);
        game.AddPlayer(Guid.NewGuid(), request.HostName);

        await _gameRepository.AddAsync(game, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameStateResponse> JoinGameAsync(string roomCode, JoinGameRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.PlayerName))
            throw new ValidationException("PlayerName is required.");

        var game = await RequireByRoomCodeAsync(roomCode, cancellationToken);

        game.AddPlayer(Guid.NewGuid(), request.PlayerName);

        _gameRepository.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameStateResponse> StartGameAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await RequireByIdAsync(gameId, cancellationToken);

        game.StartGame();

        _gameRepository.Update(game);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToResponse(game);
    }

    public async Task<GameStateResponse> GetByIdAsync(Guid gameId, CancellationToken cancellationToken = default)
    {
        var game = await RequireByIdAsync(gameId, cancellationToken);
        return MapToResponse(game);
    }

    public async Task<GameStateResponse> GetByRoomCodeAsync(string roomCode, CancellationToken cancellationToken = default)
    {
        var game = await RequireByRoomCodeAsync(roomCode, cancellationToken);
        return MapToResponse(game);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private async Task<Game> RequireByIdAsync(Guid gameId, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByIdAsync(gameId, cancellationToken);
        if (game is null)
            throw new NotFoundException($"Game {gameId} was not found.");
        return game;
    }

    private async Task<Game> RequireByRoomCodeAsync(string roomCode, CancellationToken cancellationToken)
    {
        var game = await _gameRepository.GetByRoomCodeAsync(roomCode, cancellationToken);
        if (game is null)
            throw new NotFoundException($"Game with room code '{roomCode}' was not found.");
        return game;
    }

    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        return new string(Enumerable.Range(0, 6)
            .Select(_ => chars[Random.Shared.Next(chars.Length)])
            .ToArray());
    }

    // -------------------------------------------------------------------------
    // Mapping
    // -------------------------------------------------------------------------

    private static GameStateResponse MapToResponse(Game game) => new()
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
                HasLeft = p.HasLeft,
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
