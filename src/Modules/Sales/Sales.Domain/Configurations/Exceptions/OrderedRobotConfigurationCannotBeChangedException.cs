using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Sales.Domain.Configurations.Exceptions;

/// <summary>Thrown when a configuration that already became an order is changed or ordered again (rule S1).</summary>
public sealed class OrderedRobotConfigurationCannotBeChangedException(RobotConfigurationId configurationId)
    : DomainException("S1", $"Konfiguracja {configurationId.Value} została już zamówiona i nie można jej zmienić.");
