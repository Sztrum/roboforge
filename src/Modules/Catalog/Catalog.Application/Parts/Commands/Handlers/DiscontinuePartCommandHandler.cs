using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Catalog.Application.Database;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts;

namespace RoboForge.Modules.Catalog.Application.Parts.Commands.Handlers;

public sealed class DiscontinuePartCommandHandler(ICatalogDbContext dbContext) : ICommandHandler<DiscontinuePartCommand, PartResource>
{
    public async ValueTask<PartResource> Handle(DiscontinuePartCommand command, CancellationToken cancellationToken)
    {
        var part = await dbContext.Parts.FindAsync([new PartId(command.PartId)], cancellationToken);
        Guard.Against.NotFound(command.PartId, part);

        part.Discontinue();
        await dbContext.SaveChangesAsync(cancellationToken);

        return PartResource.FromAggregate(part);
    }
}
