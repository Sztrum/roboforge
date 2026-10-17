using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Orders;
using RoboForge.Modules.Sales.Domain.Orders.ValueObjects;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Infrastructure.Persistence;

namespace RoboForge.Modules.Sales.Infrastructure.Database.Configurations;

internal sealed class OrderEntityTypeConfiguration : IEntityTypeConfiguration<OrderAggregate>
{
    public void Configure(EntityTypeBuilder<OrderAggregate> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);
        builder.Property(order => order.Id)
            .HasConversion(orderId => orderId.Value, value => new OrderId(value))
            .ValueGeneratedNever();

        builder.Property(order => order.ConfigurationId)
            .HasConversion(configurationId => configurationId.Value, value => new RobotConfigurationId(value));
        builder.HasIndex(order => order.ConfigurationId).IsUnique();

        builder.Property(order => order.Status).HasConversion<string>().HasMaxLength(20);
        builder.HasIndex(order => order.PlacedOnUtc);

        builder.Property(order => order.Price).StoreAsJson();
        builder.ComplexProperty(order => order.LeadTime, leadTime => leadTime.Property(lead => lead.WorkingDays));

        builder.Property(order => order.TrackingNumber).HasMaxLength(100);
        builder.Property(order => order.CancellationReason).HasMaxLength(500);

        builder.Ignore(order => order.Parts);
        builder.Property<List<PartSnapshot>>("_parts").HasColumnName("Parts").StoreListAsJson();

        builder.Ignore(order => order.CompletedStages);
        builder.Property<List<OrderStageEntry>>("_completedStages").HasColumnName("CompletedStages").StoreListAsJson();

        builder.Ignore(order => order.Notes);
        builder.Property<List<OrderNote>>("_notes").HasColumnName("Notes").StoreListAsJson();

        builder.Ignore(order => order.DomainEvents);
    }
}
