using Microsoft.AspNetCore.Mvc;
using Yatzy.Application.DTOs;
using Yatzy.Application.Interfaces;

namespace Yatzy.Api.Controllers;

[ApiController]
[Route("api/games")]
public sealed class GamesController : ControllerBase
{
    private readonly IGameAppService _gameAppService;
    private readonly IGameHubService _hubService;

    public GamesController(IGameAppService gameAppService, IGameHubService hubService)
    {
        _gameAppService = gameAppService;
        _hubService = hubService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateGame(
        [FromBody] CreateGameRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _gameAppService.CreateGameAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { gameId = result.GameId }, result);
    }

    [HttpPost("{roomCode}/join")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> JoinGame(
        string roomCode,
        [FromBody] JoinGameRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _gameAppService.JoinGameAsync(roomCode, request, cancellationToken);
        await _hubService.BroadcastPlayerJoinedAsync(result.RoomCode, result);
        return Ok(result);
    }

    [HttpPost("{gameId:guid}/start")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> StartGame(
        Guid gameId,
        CancellationToken cancellationToken)
    {
        var result = await _gameAppService.StartGameAsync(gameId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("{gameId:guid}")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(
        Guid gameId,
        CancellationToken cancellationToken)
    {
        var result = await _gameAppService.GetByIdAsync(gameId, cancellationToken);
        return Ok(result);
    }

    [HttpGet("by-room/{roomCode}")]
    [ProducesResponseType(typeof(GameStateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByRoomCode(
        string roomCode,
        CancellationToken cancellationToken)
    {
        var result = await _gameAppService.GetByRoomCodeAsync(roomCode, cancellationToken);
        return Ok(result);
    }
}
