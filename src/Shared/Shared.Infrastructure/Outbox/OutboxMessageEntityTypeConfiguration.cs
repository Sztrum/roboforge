using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace RoboForge.Shared.Infrastructure.Outbox;

/// <summary>Maps the outbox table; each module applies it, so the table lives in the module's schema.</summary>
public sealed class OutboxMessageEntityTypeConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");

        builder.HasKey(message => message.Id);
        builder.Property(message => message.Id).ValueGeneratedNever();
        builder.Property(message => message.Type).HasMaxLength(500);
        builder.Property(message => message.Content).HasColumnType("jsonb");

        // The publisher looks only for unprocessed messages, oldest first.
        builder.HasIndex(message => message.OccurredOnUtc).HasFilter("\"ProcessedOnUtc\" IS NULL");
    }
}
