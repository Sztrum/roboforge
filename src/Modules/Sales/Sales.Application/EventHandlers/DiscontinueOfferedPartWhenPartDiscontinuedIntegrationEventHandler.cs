using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Catalog.IntegrationEvents;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Parts;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>Marks the part as no longer offered in Sales' copy, so it cannot be added to configurations (R7).</summary>
public sealed class DiscontinueOfferedPartWhenPartDiscontinuedIntegrationEventHandler(ISalesDbContext dbContext)
    : INotificationHandler<PartDiscontinuedIntegrationEvent>
{
    public async ValueTask Handle(PartDiscontinuedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var offeredPart = await dbContext.OfferedParts.FindAsync([new CatalogPartId(notification.PartId)], cancellationToken);
        Guard.Against.NotFound(notification.PartId, offeredPart);

        offeredPart.Discontinue();
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
