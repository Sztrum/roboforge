using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Sales.IntegrationEvents;

/// <summary>A customer's order was confirmed; production of the robot should start.</summary>
public sealed record OrderConfirmedIntegrationEvent(Guid OrderId, IReadOnlyList<ConfirmedOrderPartDto> Parts) : IntegrationEvent;
