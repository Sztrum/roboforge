using Mediator;
using RoboForge.Modules.Sales.Application.Configurations.Resources;

namespace RoboForge.Modules.Sales.Application.Configurations.Commands;

/// <summary>Starts a new, empty robot configuration.</summary>
public sealed record StoreRobotConfigurationCommand : ICommand<RobotConfigurationResource>;
