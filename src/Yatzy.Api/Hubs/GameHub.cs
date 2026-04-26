using Microsoft.AspNetCore.SignalR;
using Yatzy.Application.DTOs;
using Yatzy.Application.Interfaces;
using Yatzy.Domain.Enums;
using Yatzy.Domain.Exceptions;

namespace Yatzy.Api.Hubs;

public sealed class GameHub : Hub
{
    private readonly IGameAppService _gameAppService;
    private readonly IGameplayAppService _gameplayAppService;
    private readonly IConnectionService _connectionService;
    private readonly IGameHubService _hubService;
    private readonly ILogger<GameHub> _logger;
    private readonly IHostEnvironment _env;

    public GameHub(
        IGameAppService gameAppService,
        IGameplayAppService gameplayAppService,
        IConnectionService connectionService,
        IGameHubService hubService,
        ILogger<GameHub> logger,
        IHostEnvironment env)
    {
        _gameAppService = gameAppService;
        _gameplayAppService = gameplayAppService;
        _connectionService = connectionService;
        _hubService = hubService;
        _logger = logger;
        _env = env;
    }

    // -------------------------------------------------------------------------
    // Join room
    // -------------------------------------------------------------------------

    public async Task JoinRoom(string roomCode, Guid playerId)
    {
        try
        {
            var state = await _gameAppService.GetByRoomCodeAsync(roomCode);

            var player = state.Players.FirstOrDefault(p => p.PlayerId == playerId);
            if (player is null)
            {
                await Clients.Caller.SendAsync(HubEvents.Error, $"Player {playerId} is not in game '{roomCode}'.");
                return;
            }

            _connectionService.Register(Context.ConnectionId, state.GameId, playerId, roomCode);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

            var reconnected = await _gameplayAppService.PlayerReconnectedAsync(state.GameId, playerId);
            if (reconnected is not null)
                await _hubService.BroadcastGameStateAsync(roomCode, reconnected);
            else
                await Clients.Caller.SendAsync(HubEvents.GameStateUpdated, state);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex);
        }
    }

    // -------------------------------------------------------------------------
    // Lobby
    // -------------------------------------------------------------------------

    public async Task StartGame(StartGameRequest request)
    {
        try
        {
            var entry = _connectionService.Get(Context.ConnectionId);
            if (entry is null)
            {
                await Clients.Caller.SendAsync(HubEvents.Error, "Not connected to a game.");
                return;
            }

            var (_, playerId, roomCode) = entry.Value;

            var current = await _gameAppService.GetByIdAsync(request.GameId);

            // Host = first player (index 0, JoinOrder = 0)
            if (current.Players.Count == 0 || current.Players[0].PlayerId != playerId)
            {
                await Clients.Caller.SendAsync(HubEvents.Error, "Only the host can start the game.");
                return;
            }

            var state = await _gameAppService.StartGameAsync(request.GameId);
            await _hubService.BroadcastGameStartedAsync(roomCode, state);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex);
        }
    }

    // -------------------------------------------------------------------------
    // Gameplay
    // -------------------------------------------------------------------------

    public async Task RollDice(RollDiceRequest request)
    {
        try
        {
            var (state, persistTask) = await _gameplayAppService.RollDiceFastAsync(request);

            // Broadcast til alle spillere og gem i DB parallelt
            await Task.WhenAll(
                _hubService.BroadcastDiceRolledAsync(state.RoomCode, state),
                persistTask
            );
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex);
        }
    }

    public async Task ToggleHold(ToggleHoldRequest request)
    {
        try
        {
            var state = await _gameplayAppService.ToggleHoldAsync(request);
            await _hubService.BroadcastHoldChangedAsync(state.RoomCode, state);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex);
        }
    }

    public async Task SelectScore(SelectScoreRequest request)
    {
        try
        {
            var state = await _gameplayAppService.SelectScoreAsync(request);

            if (state.Status == nameof(GameStatus.Completed))
                await _hubService.BroadcastGameEndedAsync(state.RoomCode, state);
            else
                await _hubService.BroadcastScoreSelectedAsync(state.RoomCode, state);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex);
        }
    }

    public async Task TriggerYatzy(Guid targetPlayerId, string gifName)
    {
        try
        {
            var entry = _connectionService.Get(Context.ConnectionId);
            if (entry is null) return;

            var (gameId, _, roomCode) = entry.Value;
            var game = await _gameAppService.GetByIdAsync(gameId);

            // Kun host (første spiller) må trigger
            if (game.Players.Count == 0 || game.Players[0].PlayerId != entry.Value.PlayerId)
            {
                await Clients.Caller.SendAsync(HubEvents.Error, "Only the host can trigger Yatzy celebration.");
                return;
            }

            await _hubService.BroadcastYatzyTriggerAsync(roomCode, targetPlayerId, gifName);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex);
        }
    }

    public async Task LeaveGame(LeaveGameRequest request)
    {
        try
        {
            var entry = _connectionService.Get(Context.ConnectionId);
            var state = await _gameplayAppService.LeaveGameAsync(request);

            if (entry is not null)
            {
                _connectionService.Unregister(Context.ConnectionId);
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, state.RoomCode);
            }

            if (state.Status == nameof(GameStatus.Completed))
                await _hubService.BroadcastGameEndedAsync(state.RoomCode, state);
            else
                await _hubService.BroadcastPlayerLeftAsync(state.RoomCode, state);
        }
        catch (Exception ex)
        {
            await SendErrorAsync(ex);
        }
    }

    // -------------------------------------------------------------------------
    // Disconnect
    // -------------------------------------------------------------------------

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var entry = _connectionService.Get(Context.ConnectionId);

        if (entry is not null)
        {
            var (gameId, playerId, roomCode) = entry.Value;

            _connectionService.Unregister(Context.ConnectionId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomCode);

            try
            {
                var state = await _gameplayAppService.PlayerDisconnectedAsync(gameId, playerId);
                await _hubService.BroadcastPlayerLeftAsync(roomCode, state);
            }
            catch
            {
                // best-effort — do not rethrow on disconnect
            }
        }

        await base.OnDisconnectedAsync(exception);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private Task SendErrorAsync(Exception ex)
    {
        var isDomain = ex is DomainException or NotFoundException or ValidationException;
        var message = isDomain ? ex.Message : "An unexpected error occurred.";

        // Always log unexpected errors; in development also send detail to client
        if (!isDomain)
        {
            _logger.LogError(ex, "Unhandled exception in GameHub");
            if (_env.IsDevelopment())
                message = $"{ex.GetType().Name}: {ex.Message}";
        }

        return Clients.Caller.SendAsync(HubEvents.Error, message);
    }
}
