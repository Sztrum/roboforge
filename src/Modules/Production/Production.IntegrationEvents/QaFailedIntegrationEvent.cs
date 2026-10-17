using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Production.IntegrationEvents;

/// <summary>An order's robot failed a quality check and goes back to assembly; it may now be on hold.</summary>
public sealed record QaFailedIntegrationEvent(
    Guid JobId,
    Guid OrderId,
    string Reason,
    int FailureCount,
    bool IsOnHold,
    DateTime FailedOnUtc) : IntegrationEvent;
