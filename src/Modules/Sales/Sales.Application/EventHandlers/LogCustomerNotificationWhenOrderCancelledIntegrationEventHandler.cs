using Mediator;
using Microsoft.Extensions.Logging;
using RoboForge.Modules.Sales.IntegrationEvents;

namespace RoboForge.Modules.Sales.Application.EventHandlers;

/// <summary>
/// Stands in for the customer e-mail about a cancelled order, which is out of the MVP scope
/// (docs/PROJECT.md): the notification is written to the log instead.
/// </summary>
public sealed partial class LogCustomerNotificationWhenOrderCancelledIntegrationEventHandler(
    ILogger<LogCustomerNotificationWhenOrderCancelledIntegrationEventHandler> logger)
    : INotificationHandler<OrderCancelledIntegrationEvent>
{
    public ValueTask Handle(OrderCancelledIntegrationEvent notification, CancellationToken cancellationToken)
    {
        LogCustomerNotified(notification.OrderId, notification.Reason);

        return ValueTask.CompletedTask;
    }

    [LoggerMessage(Level = LogLevel.Information, Message = "Customer notification: order {OrderId} was cancelled ({Reason})")]
    private partial void LogCustomerNotified(Guid orderId, string reason);
}
