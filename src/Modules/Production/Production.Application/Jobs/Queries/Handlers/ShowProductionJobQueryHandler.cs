using Ardalis.GuardClauses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs;

namespace RoboForge.Modules.Production.Application.Jobs.Queries.Handlers;

public sealed class ShowProductionJobQueryHandler(IProductionDbContext dbContext) : IQueryHandler<ShowProductionJobQuery, ProductionJobResource>
{
    public async ValueTask<ProductionJobResource> Handle(ShowProductionJobQuery query, CancellationToken cancellationToken)
    {
        var jobId = new ProductionJobId(query.JobId);
        var job = await dbContext.ProductionJobs.AsNoTracking().FirstOrDefaultAsync(job => job.Id == jobId, cancellationToken);
        Guard.Against.NotFound(query.JobId, job);

        return ProductionJobResource.FromAggregate(job);
    }
}
