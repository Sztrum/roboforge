using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;

namespace RoboForge.Modules.Sales.Application.Configurations.Queries;

/// <summary>Shows one robot configuration with its parts.</summary>
public sealed record ShowRobotConfigurationQuery(Guid ConfigurationId) : IQuery<RobotConfigurationResource>;
