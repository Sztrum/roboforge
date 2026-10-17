using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Catalog.IntegrationEvents;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>Updates the price in Sales' copy of the offer; configurations keep their snapshots.</summary>
public sealed class ChangeOfferedPartPriceWhenPartPriceChangedIntegrationEventHandler(ISalesDbContext dbContext)
    : INotificationHandler<PartPriceChangedIntegrationEvent>
{
    public async ValueTask Handle(PartPriceChangedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var offeredPart = await dbContext.OfferedParts.FindAsync([new CatalogPartId(notification.PartId)], cancellationToken);
        Guard.Against.NotFound(notification.PartId, offeredPart);

        offeredPart.ChangePrice(new Money(notification.PriceAmount, notification.PriceCurrency));
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
