using Microsoft.EntityFrameworkCore;
using Yatzy.Persistence.Entities;

namespace Yatzy.Persistence;

public sealed class YatzyDbContext : DbContext
{
    public YatzyDbContext(DbContextOptions<YatzyDbContext> options) : base(options) { }

    public DbSet<GameEntity> Games => Set<GameEntity>();
    public DbSet<PlayerEntity> Players => Set<PlayerEntity>();
    public DbSet<DiceEntity> Dice => Set<DiceEntity>();
    public DbSet<ScoreEntryEntity> ScoreEntries => Set<ScoreEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(YatzyDbContext).Assembly);
    }
}
