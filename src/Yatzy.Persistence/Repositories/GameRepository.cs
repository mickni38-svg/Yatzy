using Microsoft.EntityFrameworkCore;
using Yatzy.Application.Interfaces;
using Yatzy.Domain.Entities;
using Yatzy.Persistence.Entities;

namespace Yatzy.Persistence.Repositories;

public sealed class GameRepository : IGameRepository
{
    private readonly YatzyDbContext _context;

    public GameRepository(YatzyDbContext context)
    {
        _context = context;
    }

    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await QueryFull()
            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task<Game?> GetByRoomCodeAsync(string roomCode, CancellationToken cancellationToken = default)
    {
        var entity = await QueryFull()
            .FirstOrDefaultAsync(g => g.RoomCode == roomCode.ToUpperInvariant(), cancellationToken);

        return entity is null ? null : MapToDomain(entity);
    }

    public async Task AddAsync(Game game, CancellationToken cancellationToken = default)
    {
        var entity = MapToEntity(game);
        await _context.Games.AddAsync(entity, cancellationToken);
    }

    public void Update(Game game)
    {
        var entity = MapToEntity(game);

        // Detach any already-tracked instance to avoid duplicate tracking
        DetachIfTracked<GameEntity>(entity.Id);
        foreach (var p in entity.Players)
        {
            DetachIfTracked<PlayerEntity>(p.Id);
            foreach (var s in p.ScoreEntries)
                DetachIfTracked<ScoreEntryEntity>(s.Id);
        }
        foreach (var d in entity.Dice)
            DetachIfTracked<DiceEntity>(d.Id);

        // Attach the full graph; EF will track all navigation properties
        _context.Games.Attach(entity);

        // Mark root and players as modified
        _context.Entry(entity).State = EntityState.Modified;
        foreach (var p in entity.Players)
        {
            // New players (just joined) are Added; existing ones are Modified
            bool isNew = !_context.Players.Any(x => x.Id == p.Id);
            _context.Entry(p).State = isNew ? EntityState.Added : EntityState.Modified;

            foreach (var s in p.ScoreEntries)
            {
                bool isNewScore = !_context.ScoreEntries.Any(x => x.Id == s.Id);
                if (isNewScore)
                    _context.Entry(s).State = EntityState.Added;
                // existing score entries are immutable — leave as Unchanged
            }
        }
        foreach (var d in entity.Dice)
        {
            bool isNew = !_context.Dice.Any(x => x.Id == d.Id);
            _context.Entry(d).State = isNew ? EntityState.Added : EntityState.Modified;
        }
    }

    private void DetachIfTracked<T>(Guid id) where T : class
    {
        var local = _context.Set<T>().Local.FirstOrDefault(e =>
            e.GetType().GetProperty("Id")?.GetValue(e) is Guid gid && gid == id);
        if (local is not null)
            _context.Entry(local).State = EntityState.Detached;
    }

    // -------------------------------------------------------------------------
    // Query helper
    // -------------------------------------------------------------------------

    private IQueryable<GameEntity> QueryFull() =>
        _context.Games
            .Include(g => g.Players)
                .ThenInclude(p => p.ScoreEntries)
            .Include(g => g.Dice)
            .AsNoTracking();

    // -------------------------------------------------------------------------
    // Mapping: Entity → Domain
    // -------------------------------------------------------------------------

    private static Game MapToDomain(GameEntity entity)
    {
        return Game.Restore(
            id: entity.Id,
            roomCode: entity.RoomCode,
            status: entity.Status,
            currentPlayerIndex: entity.CurrentPlayerIndex,
            roundNumber: entity.RoundNumber,
            rollNumber: entity.RollNumber,
            players: entity.Players
                .OrderBy(p => p.JoinOrder)
                .Select(p => Player.Restore(
                    id: p.Id,
                    displayName: p.DisplayName,
                    isConnected: p.IsConnected,
                    hasLeft: p.HasLeft,
                    scoreEntries: p.ScoreEntries.ToDictionary(
                        s => s.Category,
                        s => s.Points)))
                .ToList(),
            dice: entity.Dice
                .OrderBy(d => d.Position)
                .Select(d => Dice.Restore(d.Position, d.Value, d.IsHeld))
                .ToList());
    }

    // -------------------------------------------------------------------------
    // Mapping: Domain → Entity
    // -------------------------------------------------------------------------

    private static GameEntity MapToEntity(Game game)
    {
        var now = DateTime.UtcNow;

        return new GameEntity
        {
            Id = game.Id,
            RoomCode = game.RoomCode,
            Status = game.Status,
            CurrentPlayerIndex = game.CurrentPlayerIndex,
            RoundNumber = game.RoundNumber,
            RollNumber = game.RollNumber,
            CreatedUtc = now,
            UpdatedUtc = now,
            Players = game.Players
                .Select((p, i) => new PlayerEntity
                {
                    Id = p.Id,
                    GameId = game.Id,
                    DisplayName = p.DisplayName,
                    IsConnected = p.IsConnected,
                    HasLeft = p.HasLeft,
                    JoinOrder = i,
                    ScoreEntries = p.ScoreSheet.Entries
                        .Where(kvp => kvp.Value.IsUsed)
                        .Select(kvp => new ScoreEntryEntity
                        {
                            Id = DeterministicGuid(p.Id, (int)kvp.Key),
                            PlayerId = p.Id,
                            Category = kvp.Key,
                            Points = kvp.Value.Score!.Value
                        })
                        .ToList()
                })
                .ToList(),
            Dice = game.Dice
                .Select(d => new DiceEntity
                {
                    Id = DeterministicGuid(game.Id, d.Position),
                    GameId = game.Id,
                    Position = d.Position,
                    Value = d.Value,
                    IsHeld = d.IsHeld
                })
                .ToList()
        };
    }

    /// <summary>Creates a stable Guid from a base Guid + a small integer discriminator.</summary>
    private static Guid DeterministicGuid(Guid baseId, int discriminator)
    {
        var bytes = baseId.ToByteArray();
        bytes[14] ^= (byte)(discriminator & 0xFF);
        bytes[15] ^= (byte)((discriminator >> 8) & 0xFF);
        return new Guid(bytes);
    }
}
