using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Application.Orders.Resources;

namespace RoboForge.Modules.Sales.Application.Orders.Queries.Handlers;

public sealed class IndexOrderQueryHandler(ISalesDbContext dbContext) : IQueryHandler<IndexOrderQuery, CollectionOrderResource>
{
    private const int MaxOrdersListed = 100;

    public async ValueTask<CollectionOrderResource> Handle(IndexOrderQuery query, CancellationToken cancellationToken)
    {
        var orders = await dbContext.Orders.AsNoTracking()
            .OrderByDescending(order => order.PlacedOnUtc)
            .Take(MaxOrdersListed)
            .ToListAsync(cancellationToken);

        return new CollectionOrderResource([.. orders.Select(OrderResource.FromAggregate)]);
    }
}
