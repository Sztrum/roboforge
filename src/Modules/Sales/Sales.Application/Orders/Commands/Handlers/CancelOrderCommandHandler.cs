using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Application.Orders.Resources;
using RoboForge.Modules.Sales.Domain.Orders;

namespace RoboForge.Modules.Sales.Application.Orders.Commands.Handlers;

public sealed class CancelOrderCommandHandler(ISalesDbContext dbContext) : ICommandHandler<CancelOrderCommand, OrderResource>
{
    public async ValueTask<OrderResource> Handle(CancelOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders.FindAsync([new OrderId(command.OrderId)], cancellationToken);
        Guard.Against.NotFound(command.OrderId, order);

        order.Cancel(command.Reason.Trim());
        await dbContext.SaveChangesAsync(cancellationToken);

        return OrderResource.FromAggregate(order);
    }
}
