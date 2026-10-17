using Ardalis.GuardClauses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Catalog.Application.Database;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts;

namespace RoboForge.Modules.Catalog.Application.Parts.Queries.Handlers;

public sealed class ShowPartQueryHandler(ICatalogDbContext dbContext) : IQueryHandler<ShowPartQuery, PartResource>
{
    public async ValueTask<PartResource> Handle(ShowPartQuery query, CancellationToken cancellationToken)
    {
        var partId = new PartId(query.PartId);
        var part = await dbContext.Parts.AsNoTracking().FirstOrDefaultAsync(part => part.Id == partId, cancellationToken);
        Guard.Against.NotFound(query.PartId, part);

        return PartResource.FromAggregate(part);
    }
}
