using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Production.IntegrationEvents;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Orders;
using RoboForge.Modules.Sales.Domain.Orders.Enums;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>Shows the customer that the workshop completed a stage of their robot.</summary>
public sealed class MarkOrderStageReachedWhenProductionStageCompletedIntegrationEventHandler(ISalesDbContext dbContext)
    : INotificationHandler<ProductionStageCompletedIntegrationEvent>
{
    public async ValueTask Handle(ProductionStageCompletedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FindAsync([new OrderId(notification.OrderId)], cancellationToken);
        Guard.Against.NotFound(notification.OrderId, order);

        order.MarkStageReached(Enum.Parse<ProductionStageEnum>(notification.Stage), notification.CompletedOnUtc);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
