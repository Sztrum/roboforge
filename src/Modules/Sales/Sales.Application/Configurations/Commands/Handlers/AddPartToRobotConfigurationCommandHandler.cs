using Ardalis.GuardClauses;
using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Parts;

namespace RoboForge.Modules.Sales.Application.Configurations.Commands.Handlers;

public sealed class AddPartToRobotConfigurationCommandHandler(ISalesDbContext dbContext)
    : ICommandHandler<AddPartToRobotConfigurationCommand, RobotConfigurationResource>
{
    public async ValueTask<RobotConfigurationResource> Handle(AddPartToRobotConfigurationCommand command, CancellationToken cancellationToken)
    {
        var configuration = await dbContext.RobotConfigurations.FindAsync([new RobotConfigurationId(command.ConfigurationId)], cancellationToken);
        Guard.Against.NotFound(command.ConfigurationId, configuration);

        var offeredPart = await dbContext.OfferedParts.FindAsync([new CatalogPartId(command.PartId)], cancellationToken);
        Guard.Against.NotFound(command.PartId, offeredPart);

        configuration.AddPart(offeredPart.TakeSnapshot());
        await dbContext.SaveChangesAsync(cancellationToken);

        return RobotConfigurationResource.FromAggregate(configuration);
    }
}
