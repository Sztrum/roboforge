using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Production.IntegrationEvents;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Orders;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>Marks the order shipped and gives the customer the tracking number.</summary>
public sealed class MarkOrderShippedWhenRobotShippedIntegrationEventHandler(ISalesDbContext dbContext)
    : INotificationHandler<RobotShippedIntegrationEvent>
{
    public async ValueTask Handle(RobotShippedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FindAsync([new OrderId(notification.OrderId)], cancellationToken);
        Guard.Against.NotFound(notification.OrderId, order);

        order.MarkShipped(notification.TrackingNumber, notification.ShippedOnUtc);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
