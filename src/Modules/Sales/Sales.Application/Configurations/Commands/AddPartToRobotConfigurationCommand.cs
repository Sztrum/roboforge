using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;

namespace RoboForge.Modules.Sales.Application.Configurations.Commands;

/// <summary>Adds a snapshot of an offered part to a draft configuration.</summary>
public sealed record AddPartToRobotConfigurationCommand(Guid ConfigurationId, Guid PartId) : ICommand<RobotConfigurationResource>;
