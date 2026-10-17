using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Configurations;

namespace RoboForge.Modules.Sales.Application.Configurations.Commands.Handlers;

public sealed class StoreRobotConfigurationCommandHandler(ISalesDbContext dbContext)
    : ICommandHandler<StoreRobotConfigurationCommand, RobotConfigurationResource>
{
    public async ValueTask<RobotConfigurationResource> Handle(StoreRobotConfigurationCommand command, CancellationToken cancellationToken)
    {
        var configuration = RobotConfigurationAggregate.Create();

        dbContext.RobotConfigurations.Add(configuration);
        await dbContext.SaveChangesAsync(cancellationToken);

        return RobotConfigurationResource.FromAggregate(configuration);
    }
}
