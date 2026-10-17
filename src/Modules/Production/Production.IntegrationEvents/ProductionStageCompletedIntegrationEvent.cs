using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Production.IntegrationEvents;

/// <summary>The workshop completed a stage of an order's robot. The stage travels as its name.</summary>
public sealed record ProductionStageCompletedIntegrationEvent(
    Guid JobId,
    Guid OrderId,
    string Stage,
    string CompletedBy,
    DateTime CompletedOnUtc) : IntegrationEvent;
