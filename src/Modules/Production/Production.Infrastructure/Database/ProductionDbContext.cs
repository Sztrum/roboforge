using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Domain.Jobs;
using RoboForge.Shared.Infrastructure.Outbox;

namespace RoboForge.Modules.Production.Infrastructure.Database;

/// <summary>The Production module's own database context; its tables live in the <c>production</c> schema.</summary>
public sealed class ProductionDbContext(DbContextOptions<ProductionDbContext> options) : DbContext(options), IProductionDbContext
{
    public const string Schema = "production";

    public DbSet<ProductionJobAggregate> ProductionJobs => Set<ProductionJobAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ProductionDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
    }
}
