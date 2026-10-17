using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Orders;
using RoboForge.Modules.Sales.Domain.Parts;

namespace RoboForge.Modules.Sales.Application.Database;

/// <summary>What Sales handlers may use from the module's database: its tables and saving.</summary>
public interface ISalesDbContext
{
    DbSet<OfferedPartAggregate> OfferedParts { get; }

    DbSet<RobotConfigurationAggregate> RobotConfigurations { get; }

    DbSet<OrderAggregate> Orders { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
