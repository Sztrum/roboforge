using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Shared.Domain.Events;

namespace RoboForge.Modules.Production.Domain.Jobs.Events;

public sealed record ProductionStageCompletedEvent(
    ProductionJobId JobId,
    SalesOrderId OrderId,
    ProductionStageEnum Stage,
    string CompletedBy,
    DateTime CompletedOnUtc) : IDomainEvent;
