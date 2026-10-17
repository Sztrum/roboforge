using RoboForge.Modules.Catalog.Domain.Parts.Events;
using RoboForge.Modules.Catalog.IntegrationEvents;
using RoboForge.Shared.Domain.Events;
using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Catalog.Application.IntegrationEvents;

/// <summary>Translates Catalog domain events into the integration events other modules subscribe to.</summary>
public static class CatalogDomainEventToIntegrationEventMapper
{
    public static IntegrationEvent? Map(IDomainEvent domainEvent) => domainEvent switch
    {
        PartCreatedEvent created => new PartCreatedIntegrationEvent(
            created.PartId.Value,
            created.Name,
            created.Category.ToString(),
            created.Price.Amount,
            created.Price.Currency,
            created.LeadTime.WorkingDays,
            created.TechSpec.WeightGrams,
            created.TechSpec.MountType?.ToString(),
            created.TechSpec.VoltageVolts,
            created.TechSpec.MaxLoadGrams),
        PartPriceChangedEvent priceChanged => new PartPriceChangedIntegrationEvent(
            priceChanged.PartId.Value,
            priceChanged.NewPrice.Amount,
            priceChanged.NewPrice.Currency),
        PartDiscontinuedEvent discontinued => new PartDiscontinuedIntegrationEvent(discontinued.PartId.Value),
        _ => null,
    };
}
