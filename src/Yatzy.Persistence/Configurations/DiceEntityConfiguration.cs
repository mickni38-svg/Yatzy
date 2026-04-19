using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yatzy.Persistence.Entities;

namespace Yatzy.Persistence.Configurations;

public sealed class DiceEntityConfiguration : IEntityTypeConfiguration<DiceEntity>
{
    public void Configure(EntityTypeBuilder<DiceEntity> builder)
    {
        builder.ToTable("Dice");

        builder.HasKey(d => d.Id);

        builder.Property(d => d.Position).IsRequired();
        builder.Property(d => d.Value).IsRequired();
        builder.Property(d => d.IsHeld).IsRequired();
    }
}
