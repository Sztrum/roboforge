using Microsoft.EntityFrameworkCore;

namespace RoboForge.Modules.Catalog.Infrastructure.Database;

/// <summary>
/// The Catalog module's own database context. Its tables live in the <c>catalog</c> schema,
/// so no other module shares them.
/// </summary>
public sealed class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
{
    public const string Schema = "catalog";

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
