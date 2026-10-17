using RoboForge.Modules.Sales.Domain.Orders.Enums;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Sales.Domain.Orders.Exceptions;

/// <summary>Thrown when production progress is recorded for an order that is not being produced (rule S5).</summary>
public sealed class OrderIsNotInProductionException(OrderId orderId, OrderStatusEnum status)
    : DomainException("S5", $"Zamówienie {orderId.Value} nie jest w produkcji (status {status}).");
