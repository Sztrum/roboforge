using Ardalis.GuardClauses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Application.Orders.Resources;
using RoboForge.Modules.Sales.Domain.Orders;

namespace RoboForge.Modules.Sales.Application.Orders.Queries.Handlers;

public sealed class ShowOrderQueryHandler(ISalesDbContext dbContext) : IQueryHandler<ShowOrderQuery, OrderResource>
{
    public async ValueTask<OrderResource> Handle(ShowOrderQuery query, CancellationToken cancellationToken)
    {
        var orderId = new OrderId(query.OrderId);
        var order = await dbContext.Orders.AsNoTracking().FirstOrDefaultAsync(order => order.Id == orderId, cancellationToken);
        Guard.Against.NotFound(query.OrderId, order);

        return OrderResource.FromAggregate(order);
    }
}
