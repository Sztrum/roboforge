using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Shared.Infrastructure.Persistence;

namespace RoboForge.Modules.Sales.Infrastructure.Database.Configurations;

internal sealed class OfferedPartEntityTypeConfiguration : IEntityTypeConfiguration<OfferedPartAggregate>
{
    public void Configure(EntityTypeBuilder<OfferedPartAggregate> builder)
    {
        builder.ToTable("OfferedParts");

        builder.HasKey(part => part.Id);
        builder.Property(part => part.Id)
            .HasConversion(partId => partId.Value, value => new CatalogPartId(value))
            .ValueGeneratedNever();

        builder.Property(part => part.CurrentOffer).StoreAsJson();

        builder.Ignore(part => part.DomainEvents);
    }
}
