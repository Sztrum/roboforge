using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;

namespace RoboForge.Modules.Sales.Application.Configurations.Commands;

/// <summary>Removes one piece of a part from a draft configuration.</summary>
public sealed record RemovePartFromRobotConfigurationCommand(Guid ConfigurationId, Guid PartId) : ICommand<RobotConfigurationResource>;
