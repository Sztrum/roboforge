using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Production.IntegrationEvents;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Orders;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>Adds a note about the failed quality check to the order and extends its lead time (S6).</summary>
public sealed class RecordFailedQaOnOrderWhenQaFailedIntegrationEventHandler(ISalesDbContext dbContext)
    : INotificationHandler<QaFailedIntegrationEvent>
{
    public async ValueTask Handle(QaFailedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FindAsync([new OrderId(notification.OrderId)], cancellationToken);
        Guard.Against.NotFound(notification.OrderId, order);

        order.RecordFailedQa(notification.Reason, notification.FailedOnUtc);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
