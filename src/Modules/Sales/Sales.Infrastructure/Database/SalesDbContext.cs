using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Orders;
using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Shared.Infrastructure.Outbox;

namespace RoboForge.Modules.Sales.Infrastructure.Database;

/// <summary>The Sales module's own database context; its tables live in the <c>sales</c> schema.</summary>
public sealed class SalesDbContext(DbContextOptions<SalesDbContext> options) : DbContext(options), ISalesDbContext
{
    public const string Schema = "sales";

    public DbSet<OfferedPartAggregate> OfferedParts => Set<OfferedPartAggregate>();

    public DbSet<RobotConfigurationAggregate> RobotConfigurations => Set<RobotConfigurationAggregate>();

    public DbSet<OrderAggregate> Orders => Set<OrderAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SalesDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
    }
}
