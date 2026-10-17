using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Parts;

namespace RoboForge.Modules.Sales.Application.Configurations.Commands.Handlers;

public sealed class RemovePartFromRobotConfigurationCommandHandler(ISalesDbContext dbContext)
    : ICommandHandler<RemovePartFromRobotConfigurationCommand, RobotConfigurationResource>
{
    public async ValueTask<RobotConfigurationResource> Handle(RemovePartFromRobotConfigurationCommand command, CancellationToken cancellationToken)
    {
        var configuration = await dbContext.RobotConfigurations.FindAsync([new RobotConfigurationId(command.ConfigurationId)], cancellationToken);
        Guard.Against.NotFound(command.ConfigurationId, configuration);

        configuration.RemovePart(new CatalogPartId(command.PartId));
        await dbContext.SaveChangesAsync(cancellationToken);

        return RobotConfigurationResource.FromAggregate(configuration);
    }
}
