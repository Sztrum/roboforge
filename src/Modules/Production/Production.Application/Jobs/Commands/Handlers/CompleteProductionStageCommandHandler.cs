using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs;

namespace RoboForge.Modules.Production.Application.Jobs.Commands.Handlers;

public sealed class CompleteProductionStageCommandHandler(IProductionDbContext dbContext, TimeProvider timeProvider)
    : ICommandHandler<CompleteProductionStageCommand, ProductionJobResource>
{
    public async ValueTask<ProductionJobResource> Handle(CompleteProductionStageCommand command, CancellationToken cancellationToken)
    {
        var job = await dbContext.ProductionJobs.FindAsync([new ProductionJobId(command.JobId)], cancellationToken);
        Guard.Against.NotFound(command.JobId, job);

        job.CompleteStage(command.Stage, command.CompletedBy.Trim(), timeProvider.GetUtcNow().UtcDateTime);
        await dbContext.SaveChangesAsync(cancellationToken);

        return ProductionJobResource.FromAggregate(job);
    }
}
