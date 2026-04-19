using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yatzy.Persistence.Entities;

namespace Yatzy.Persistence.Configurations;

public sealed class PlayerEntityConfiguration : IEntityTypeConfiguration<PlayerEntity>
{
    public void Configure(EntityTypeBuilder<PlayerEntity> builder)
    {
        builder.ToTable("Players");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.DisplayName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(p => p.IsConnected).IsRequired();
        builder.Property(p => p.JoinOrder).IsRequired();

        builder.HasMany(p => p.ScoreEntries)
            .WithOne(s => s.Player)
            .HasForeignKey(s => s.PlayerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
