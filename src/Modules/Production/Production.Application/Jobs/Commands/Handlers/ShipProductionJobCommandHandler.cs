using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs;

namespace RoboForge.Modules.Production.Application.Jobs.Commands.Handlers;

public sealed class ShipProductionJobCommandHandler(IProductionDbContext dbContext, TimeProvider timeProvider)
    : ICommandHandler<ShipProductionJobCommand, ProductionJobResource>
{
    public async ValueTask<ProductionJobResource> Handle(ShipProductionJobCommand command, CancellationToken cancellationToken)
    {
        var job = await dbContext.ProductionJobs.FindAsync([new ProductionJobId(command.JobId)], cancellationToken);
        Guard.Against.NotFound(command.JobId, job);

        job.Ship(command.TrackingNumber.Trim(), timeProvider.GetUtcNow().UtcDateTime);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ProductionJobResource.FromAggregate(job);
    }
}
