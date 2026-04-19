namespace Yatzy.Application.Interfaces;

public interface IConnectionService
{
    void Register(string connectionId, Guid gameId, Guid playerId, string roomCode);
    void Unregister(string connectionId);
    (Guid GameId, Guid PlayerId, string RoomCode)? Get(string connectionId);
}
