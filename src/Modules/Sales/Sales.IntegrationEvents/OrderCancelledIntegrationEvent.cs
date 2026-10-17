using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Sales.IntegrationEvents;

/// <summary>A customer cancelled an order before production started.</summary>
public sealed record OrderCancelledIntegrationEvent(Guid OrderId, string Reason) : IntegrationEvent;
