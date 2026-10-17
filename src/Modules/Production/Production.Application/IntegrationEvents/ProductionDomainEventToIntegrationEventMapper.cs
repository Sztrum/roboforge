using RoboForge.Modules.Production.Domain.Jobs.Events;
using RoboForge.Modules.Production.IntegrationEvents;
using RoboForge.Shared.Domain.Events;
using RoboForge.Shared.IntegrationEvents;

namespace RoboForge.Modules.Production.Application.IntegrationEvents;

/// <summary>Translates Production domain events into the integration events Sales subscribes to.</summary>
public static class ProductionDomainEventToIntegrationEventMapper
{
    public static IntegrationEvent? Map(IDomainEvent domainEvent) => domainEvent switch
    {
        ProductionStageCompletedEvent completed => new ProductionStageCompletedIntegrationEvent(
            completed.JobId.Value, completed.OrderId.Value, completed.Stage.ToString(), completed.CompletedBy, completed.CompletedOnUtc),
        QaFailedEvent failed => new QaFailedIntegrationEvent(
            failed.JobId.Value, failed.OrderId.Value, failed.Reason, failed.FailureCount, failed.IsOnHold, failed.FailedOnUtc),
        RobotShippedEvent shipped => new RobotShippedIntegrationEvent(
            shipped.JobId.Value, shipped.OrderId.Value, shipped.TrackingNumber, shipped.ShippedOnUtc),
        _ => null,
    };
}
