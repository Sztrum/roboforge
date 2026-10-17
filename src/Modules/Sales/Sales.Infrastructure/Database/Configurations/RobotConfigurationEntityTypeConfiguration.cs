using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Infrastructure.Persistence;

namespace RoboForge.Modules.Sales.Infrastructure.Database.Configurations;

internal sealed class RobotConfigurationEntityTypeConfiguration : IEntityTypeConfiguration<RobotConfigurationAggregate>
{
    public void Configure(EntityTypeBuilder<RobotConfigurationAggregate> builder)
    {
        builder.ToTable("RobotConfigurations");

        builder.HasKey(configuration => configuration.Id);
        builder.Property(configuration => configuration.Id)
            .HasConversion(configurationId => configurationId.Value, value => new RobotConfigurationId(value))
            .ValueGeneratedNever();

        builder.Property(configuration => configuration.Status).HasConversion<string>().HasMaxLength(20);

        builder.Ignore(configuration => configuration.Parts);
        builder.Property<List<PartSnapshot>>("_parts").HasColumnName("Parts").StoreListAsJson();

        builder.Ignore(configuration => configuration.DomainEvents);
    }
}
