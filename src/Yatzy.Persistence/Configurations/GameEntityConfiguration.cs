using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yatzy.Persistence.Entities;

namespace Yatzy.Persistence.Configurations;

public sealed class GameEntityConfiguration : IEntityTypeConfiguration<GameEntity>
{
    public void Configure(EntityTypeBuilder<GameEntity> builder)
    {
        builder.ToTable("Games");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.RoomCode)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(g => g.RoomCode)
            .IsUnique();

        builder.Property(g => g.Status)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(g => g.CurrentPlayerIndex).IsRequired();
        builder.Property(g => g.RoundNumber).IsRequired();
        builder.Property(g => g.RollNumber).IsRequired();

        builder.Property(g => g.CreatedUtc).IsRequired();
        builder.Property(g => g.UpdatedUtc).IsRequired();

        builder.HasMany(g => g.Players)
            .WithOne(p => p.Game)
            .HasForeignKey(p => p.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(g => g.Dice)
            .WithOne(d => d.Game)
            .HasForeignKey(d => d.GameId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
