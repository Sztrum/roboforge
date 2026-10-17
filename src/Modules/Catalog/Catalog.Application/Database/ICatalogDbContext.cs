using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Catalog.Domain.Parts;

namespace RoboForge.Modules.Catalog.Application.Database;

/// <summary>What Catalog handlers may use from the module's database: its tables and saving.</summary>
public interface ICatalogDbContext
{
    DbSet<PartAggregate> Parts { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
