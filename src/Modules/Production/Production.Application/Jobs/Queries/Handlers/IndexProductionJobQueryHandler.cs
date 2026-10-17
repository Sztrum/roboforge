using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Application.Jobs.Resources;

namespace RoboForge.Modules.Production.Application.Jobs.Queries.Handlers;

public sealed class IndexProductionJobQueryHandler(IProductionDbContext dbContext)
    : IQueryHandler<IndexProductionJobQuery, CollectionProductionJobResource>
{
    private const int MaxJobsListed = 100;

    public async ValueTask<CollectionProductionJobResource> Handle(IndexProductionJobQuery query, CancellationToken cancellationToken)
    {
        var jobs = dbContext.ProductionJobs.AsNoTracking();

        if (query.Stage is { } stage)
        {
            jobs = jobs.Where(job => job.CurrentStage == stage);
        }

        var queue = await jobs
            .OrderBy(job => job.StartedOnUtc)
            .Take(MaxJobsListed)
            .ToListAsync(cancellationToken);

        return new CollectionProductionJobResource([.. queue.Select(ProductionJobResource.FromAggregate)]);
    }
}
