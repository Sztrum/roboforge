using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs;

namespace RoboForge.Modules.Production.Application.Jobs.Commands.Handlers;

public sealed class ReleaseProductionJobFromHoldCommandHandler(IProductionDbContext dbContext)
    : ICommandHandler<ReleaseProductionJobFromHoldCommand, ProductionJobResource>
{
    public async ValueTask<ProductionJobResource> Handle(ReleaseProductionJobFromHoldCommand command, CancellationToken cancellationToken)
    {
        var job = await dbContext.ProductionJobs.FindAsync([new ProductionJobId(command.JobId)], cancellationToken);
        Guard.Against.NotFound(command.JobId, job);

        job.ReleaseFromHold();
        await dbContext.SaveChangesAsync(cancellationToken);

        return ProductionJobResource.FromAggregate(job);
    }
}
