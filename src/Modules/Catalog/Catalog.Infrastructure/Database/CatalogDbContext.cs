using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Catalog.Application.Database;
using RoboForge.Modules.Catalog.Domain.Parts;
using RoboForge.Shared.Infrastructure.Outbox;

namespace RoboForge.Modules.Catalog.Infrastructure.Database;

/// <summary>
/// The Catalog module's own database context. Its tables live in the <c>catalog</c> schema,
/// so no other module shares them.
/// </summary>
public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options), ICatalogDbContext
{
    public const string Schema = "catalog";

    public DbSet<PartAggregate> Parts => Set<PartAggregate>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
        modelBuilder.ApplyConfiguration(new OutboxMessageEntityTypeConfiguration());
    }
}
