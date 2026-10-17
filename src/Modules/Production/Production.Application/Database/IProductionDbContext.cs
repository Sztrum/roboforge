using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Production.Domain.Jobs;

namespace RoboForge.Modules.Production.Application.Database;

/// <summary>What Production handlers may use from the module's database: its tables and saving.</summary>
public interface IProductionDbContext
{
    DbSet<ProductionJobAggregate> ProductionJobs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
