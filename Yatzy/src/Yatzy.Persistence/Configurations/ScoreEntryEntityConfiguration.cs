using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Yatzy.Persistence.Entities;

namespace Yatzy.Persistence.Configurations;

public sealed class ScoreEntryEntityConfiguration : IEntityTypeConfiguration<ScoreEntryEntity>
{
    public void Configure(EntityTypeBuilder<ScoreEntryEntity> builder)
    {
        builder.ToTable("ScoreEntries");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Category)
            .IsRequired()
            .HasConversion<string>()
            .HasMaxLength(30);

        builder.Property(s => s.Points).IsRequired();
    }
}
