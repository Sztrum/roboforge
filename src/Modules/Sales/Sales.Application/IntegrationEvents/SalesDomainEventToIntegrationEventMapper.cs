using RoboForge.Modules.Sales.Domain.Orders.Events;
using RoboForge.Modules.Sales.IntegrationEvents;
using RoboForge.Shared.Domain.Events;
using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Sales.Application.IntegrationEvents;

/// <summary>Translates Sales domain events into the integration events other modules subscribe to.</summary>
public static class SalesDomainEventToIntegrationEventMapper
{
    public static IntegrationEvent? Map(IDomainEvent domainEvent) => domainEvent switch
    {
        OrderConfirmedEvent confirmed => new OrderConfirmedIntegrationEvent(
            confirmed.OrderId.Value,
            [.. confirmed.Parts.Select(part => new ConfirmedOrderPartDto(part.PartId.Value, part.Name, part.Category.ToString()))]),
        OrderCancelledEvent cancelled => new OrderCancelledIntegrationEvent(cancelled.OrderId.Value, cancelled.Reason),
        _ => null,
    };
}
