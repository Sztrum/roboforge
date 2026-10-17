using Ardalis.GuardClauses;
using Mediator;
using Microsoft.EntityFrameworkCore;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Domain.Configurations;

namespace RoboForge.Modules.Sales.Application.Configurations.Queries.Handlers;

public sealed class ShowRobotConfigurationQuoteQueryHandler(ISalesDbContext dbContext)
    : IQueryHandler<ShowRobotConfigurationQuoteQuery, RobotQuoteResource>
{
    public async ValueTask<RobotQuoteResource> Handle(ShowRobotConfigurationQuoteQuery query, CancellationToken cancellationToken)
    {
        var configurationId = new RobotConfigurationId(query.ConfigurationId);
        var configuration = await dbContext.RobotConfigurations.AsNoTracking()
            .FirstOrDefaultAsync(configuration => configuration.Id == configurationId, cancellationToken);
        Guard.Against.NotFound(query.ConfigurationId, configuration);

        return RobotQuoteResource.FromQuote(configuration.PrepareQuote());
    }
}
