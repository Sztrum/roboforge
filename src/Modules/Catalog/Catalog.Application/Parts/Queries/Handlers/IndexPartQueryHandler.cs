using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Catalog.Application.Database;
using RoboForge.Modules.Catalog.Application.Parts.Resources;

namespace RoboForge.Modules.Catalog.Application.Parts.Queries.Handlers;

public sealed class IndexPartQueryHandler(ICatalogDbContext dbContext) : IQueryHandler<IndexPartQuery, CollectionPartResource>
{
    public async ValueTask<CollectionPartResource> Handle(IndexPartQuery query, CancellationToken cancellationToken)
    {
        var parts = dbContext.Parts.AsNoTracking();

        if (query.Category is { } category)
        {
            parts = parts.Where(part => part.Category == category);
        }

        var partResources = await parts
            .OrderBy(part => part.Category)
            .ThenBy(part => part.Name)
            .Select(part => new PartResource(
                part.Id.Value,
                part.Name,
                part.Category,
                part.Price.Amount,
                part.Price.Currency,
                part.LeadTime.WorkingDays,
                part.TechSpec.WeightGrams,
                part.TechSpec.MountType,
                part.TechSpec.VoltageVolts,
                part.TechSpec.MaxLoadGrams,
                part.IsDiscontinued))
            .ToListAsync(cancellationToken);

        return new CollectionPartResource(partResources);
    }
}
