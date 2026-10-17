using Ardalis.GuardClauses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Configurations;

namespace RoboForge.Modules.Sales.Application.Configurations.Queries.Handlers;

public sealed class ShowRobotConfigurationQueryHandler(ISalesDbContext dbContext)
    : IQueryHandler<ShowRobotConfigurationQuery, RobotConfigurationResource>
{
    public async ValueTask<RobotConfigurationResource> Handle(ShowRobotConfigurationQuery query, CancellationToken cancellationToken)
    {
        var configurationId = new RobotConfigurationId(query.ConfigurationId);
        var configuration = await dbContext.RobotConfigurations.AsNoTracking()
            .FirstOrDefaultAsync(configuration => configuration.Id == configurationId, cancellationToken);
        Guard.Against.NotFound(query.ConfigurationId, configuration);

        return RobotConfigurationResource.FromAggregate(configuration);
    }
}
