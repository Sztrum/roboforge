using RoboForge.Modules.Sales.Domain.Orders.Enums;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Sales.Domain.Orders.Exceptions;

/// <summary>Thrown when an order that is not waiting for confirmation is confirmed (rule S3).</summary>
public sealed class OrderCanBeConfirmedOnlyWhenPlacedException(OrderId orderId, OrderStatusEnum status)
    : DomainException("S3", $"Zamówienie {orderId.Value} można potwierdzić tylko po złożeniu (status {status}).");
