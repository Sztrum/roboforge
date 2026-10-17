using RoboForge.Shared.Domain.Events;

namespace RoboForge.Modules.Production.Domain.Jobs.Events;

public sealed record RobotShippedEvent(ProductionJobId JobId, SalesOrderId OrderId, string TrackingNumber, DateTime ShippedOnUtc) : IDomainEvent;
