using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoboForge.Modules.Catalog.Domain.Parts;

namespace RoboForge.Modules.Catalog.Infrastructure.Database.Configurations;

internal sealed class PartEntityTypeConfiguration : IEntityTypeConfiguration<PartAggregate>
{
    public void Configure(EntityTypeBuilder<PartAggregate> builder)
    {
        builder.ToTable("Parts");

        builder.HasKey(part => part.Id);
        builder.Property(part => part.Id)
            .HasConversion(partId => partId.Value, value => new PartId(value))
            .ValueGeneratedNever();

        builder.Property(part => part.Name).HasMaxLength(200);
        builder.Property(part => part.Category).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(part => part.Category);

        builder.ComplexProperty(part => part.Price, price =>
        {
            price.Property(money => money.Amount).HasPrecision(12, 2);
            price.Property(money => money.Currency).HasMaxLength(3);
        });

        builder.ComplexProperty(part => part.LeadTime, leadTime =>
            leadTime.Property(lead => lead.WorkingDays));

        builder.ComplexProperty(part => part.TechSpec, techSpec =>
        {
            techSpec.Property(spec => spec.WeightGrams);
            techSpec.Property(spec => spec.MountType).HasConversion<string>().HasMaxLength(20);
            techSpec.Property(spec => spec.VoltageVolts);
            techSpec.Property(spec => spec.MaxLoadGrams);
        });

        builder.Ignore(part => part.DomainEvents);
    }
}
