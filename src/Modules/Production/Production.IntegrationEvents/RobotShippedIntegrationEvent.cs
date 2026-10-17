using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Production.IntegrationEvents;

/// <summary>An order's robot left the workshop.</summary>
public sealed record RobotShippedIntegrationEvent(Guid JobId, Guid OrderId, string TrackingNumber, DateTime ShippedOnUtc) : IntegrationEvent;
