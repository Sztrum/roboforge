using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Domain.Jobs;
using RoboForge.Modules.Production.Domain.Jobs.ValueObjects;
using RoboForge.Modules.Sales.IntegrationEvents;

namespace RoboForge.Modules.Production.Application.EventHandlers;

/// <summary>Starts building the robot of a confirmed order; a repeated delivery starts no second job.</summary>
public sealed class StartProductionJobWhenOrderConfirmedIntegrationEventHandler(IProductionDbContext dbContext, TimeProvider timeProvider)
    : INotificationHandler<OrderConfirmedIntegrationEvent>
{
    public async ValueTask Handle(OrderConfirmedIntegrationEvent notification, CancellationToken cancellationToken)
    {
        var orderId = new SalesOrderId(notification.OrderId);

        if (await dbContext.ProductionJobs.AnyAsync(job => job.OrderId == orderId, cancellationToken))
        {
            return;
        }

        dbContext.ProductionJobs.Add(ProductionJobAggregate.StartForOrder(
            orderId,
            notification.Parts.Select(part => new ProductionPart(part.PartId, part.Name, part.Category)),
            timeProvider.GetUtcNow().UtcDateTime));

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
