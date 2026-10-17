using Mediator;
using RoboForge.Modules.Catalog.IntegrationEvents;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>Adds a new catalog part to Sales' copy of the offer; a repeated delivery changes nothing.</summary>
public sealed class CopyPartToOfferWhenPartCreatedIntegrationEventHandler(ISalesDbContext dbContext)
    : INotificationHandler<PartCreatedIntegrationEvent>
{
    public async ValueTask Handle(PartCreatedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var partId = new CatalogPartId(notification.PartId);

        if (await dbContext.OfferedParts.FindAsync([partId], cancellationToken) is not null)
        {
            return;
        }

        dbContext.OfferedParts.Add(OfferedPartAggregate.CopyFromCatalog(
            partId,
            notification.Name,
            Enum.Parse<PartCategoryEnum>(notification.Category),
            new Money(notification.PriceAmount, notification.PriceCurrency),
            new LeadTime(notification.LeadTimeWorkingDays),
            notification.WeightGrams,
            notification.MountType is null ? null : Enum.Parse<MountTypeEnum>(notification.MountType),
            notification.VoltageVolts,
            notification.MaxLoadGrams));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
