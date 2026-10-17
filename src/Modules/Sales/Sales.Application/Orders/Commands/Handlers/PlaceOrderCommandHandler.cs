using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Application.Orders.Resources;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Orders;

namespace RoboForge.Modules.Sales.Application.Orders.Commands.Handlers;

public sealed class PlaceOrderCommandHandler(ISalesDbContext dbContext, TimeProvider timeProvider)
    : ICommandHandler<PlaceOrderCommand, OrderResource>
{
    public async ValueTask<OrderResource> Handle(PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var configuration = await dbContext.RobotConfigurations.FindAsync([new RobotConfigurationId(command.ConfigurationId)], cancellationToken);
        Guard.Against.NotFound(command.ConfigurationId, configuration);

        var order = OrderAggregate.Place(configuration, timeProvider.GetUtcNow().UtcDateTime);

        dbContext.Orders.Add(order);
        await dbContext.SaveChangesAsync(cancellationToken);

        return OrderResource.FromAggregate(order);
    }
}
