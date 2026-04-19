using Yatzy.Domain.Entities;

namespace Yatzy.Application.Interfaces;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Game?> GetByRoomCodeAsync(string roomCode, CancellationToken cancellationToken = default);
    Task AddAsync(Game game, CancellationToken cancellationToken = default);
    void Update(Game game);
}
