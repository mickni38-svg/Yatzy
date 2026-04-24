using Microsoft.AspNetCore.SignalR;
using Yatzy.Application.DTOs;
using Yatzy.Application.Interfaces;
using Yatzy.Api.Hubs;

namespace Yatzy.Api.Services;

public sealed class GameHubService : IGameHubService
{
    private readonly IHubContext<GameHub> _hubContext;

    public GameHubService(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public Task BroadcastGameStateAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.GameStateUpdated, state);

    public Task BroadcastPlayerJoinedAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.PlayerJoined, state);

    public Task BroadcastPlayerLeftAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.PlayerLeft, state);

    public Task BroadcastGameStartedAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.GameStarted, state);

    public Task BroadcastDiceRolledAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.DiceRolled, state);

    public Task BroadcastHoldChangedAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.HoldChanged, state);

    public Task BroadcastScoreSelectedAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.ScoreSelected, state);

    public Task BroadcastGameEndedAsync(string roomCode, GameStateResponse state) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.GameEnded, state);

    public Task BroadcastYatzyTriggerAsync(string roomCode, Guid targetPlayerId, string gifName) =>
        _hubContext.Clients.Group(roomCode).SendAsync(HubEvents.TriggerYatzy, targetPlayerId, gifName);
}
