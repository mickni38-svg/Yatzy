using System.Collections.Concurrent;
using Yatzy.Application.Interfaces;

namespace Yatzy.Application.Services;

public sealed class ConnectionService : IConnectionService
{
    private readonly ConcurrentDictionary<string, (Guid GameId, Guid PlayerId, string RoomCode)> _connections = new();

    public void Register(string connectionId, Guid gameId, Guid playerId, string roomCode) =>
        _connections[connectionId] = (gameId, playerId, roomCode);

    public void Unregister(string connectionId) =>
        _connections.TryRemove(connectionId, out _);

    public (Guid GameId, Guid PlayerId, string RoomCode)? Get(string connectionId) =>
        _connections.TryGetValue(connectionId, out var entry) ? entry : null;
}
