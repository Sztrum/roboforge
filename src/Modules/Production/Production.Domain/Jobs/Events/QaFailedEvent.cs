using RoboForge.Shared.Domain.Events;

namespace RoboForge.Modules.Production.Domain.Jobs.Events;

public sealed record QaFailedEvent(
    ProductionJobId JobId,
    SalesOrderId OrderId,
    string Reason,
    int FailureCount,
    bool IsOnHold,
    DateTime FailedOnUtc) : IDomainEvent;
