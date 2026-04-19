using Microsoft.AspNetCore.SignalR;

namespace Yatzy.Api.Hubs;

/// <summary>
/// Signaling hub for WebRTC peer-to-peer camera streaming.
/// Clients exchange SDP offers/answers and ICE candidates through this hub.
/// </summary>
public sealed class VideoHub : Hub
{
    // room → set of connectionIds
    private static readonly Dictionary<string, HashSet<string>> _rooms = new();
    // connectionId → (roomCode, playerId)
    private static readonly Dictionary<string, (string RoomCode, string PlayerId)> _connections = new();
    private static readonly object _lock = new();

    public async Task JoinVideoRoom(string roomCode, string playerId)
    {
        lock (_lock)
        {
            _connections[Context.ConnectionId] = (roomCode, playerId);
            if (!_rooms.TryGetValue(roomCode, out var set))
            {
                set = [];
                _rooms[roomCode] = set;
            }
            set.Add(Context.ConnectionId);
        }

        await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);

        // Tell other peers in the room that a new participant arrived so they
        // can initiate an offer towards us.
        await Clients.OthersInGroup(roomCode)
            .SendAsync("PeerJoined", playerId, Context.ConnectionId);
    }

    /// <summary>Forward an SDP offer to a specific peer.</summary>
    public async Task SendOffer(string targetConnectionId, string sdp, string playerId)
    {
        await Clients.Client(targetConnectionId)
            .SendAsync("ReceiveOffer", sdp, playerId, Context.ConnectionId);
    }

    /// <summary>Forward an SDP answer back to the caller.</summary>
    public async Task SendAnswerTo(string targetConnectionId, string sdp, string playerId)
    {
        await Clients.Client(targetConnectionId)
            .SendAsync("ReceiveAnswerFrom", sdp, Context.ConnectionId);
    }

    /// <summary>Relay an ICE candidate to a specific peer.</summary>
    public async Task SendIceCandidate(string targetConnectionId, string candidate, string sdpMid, int sdpMLineIndex)
    {
        await Clients.Client(targetConnectionId)
            .SendAsync("ReceiveIceCandidate", candidate, sdpMid, sdpMLineIndex, Context.ConnectionId);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? roomCode = null;
        string? playerId = null;

        lock (_lock)
        {
            if (_connections.TryGetValue(Context.ConnectionId, out var entry))
            {
                (roomCode, playerId) = entry;
                _connections.Remove(Context.ConnectionId);
                if (_rooms.TryGetValue(roomCode, out var set))
                    set.Remove(Context.ConnectionId);
            }
        }

        if (roomCode is not null && playerId is not null)
            await Clients.OthersInGroup(roomCode).SendAsync("PeerLeft", playerId);

        await base.OnDisconnectedAsync(exception);
    }
}
