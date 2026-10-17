using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;

namespace RoboForge.Modules.Sales.Application.Configurations.Queries;

/// <summary>Validates a configuration and computes its price and lead time.</summary>
public sealed record ShowRobotConfigurationQuoteQuery(Guid ConfigurationId) : IQuery<RobotQuoteResource>;
