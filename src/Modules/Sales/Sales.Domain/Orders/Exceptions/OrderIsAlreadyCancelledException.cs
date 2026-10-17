using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Sales.Domain.Orders.Exceptions;

/// <summary>Thrown when a cancelled order is cancelled again (rule S4).</summary>
public sealed class OrderIsAlreadyCancelledException(OrderId orderId)
    : DomainException("S4", $"Zamówienie {orderId.Value} jest już anulowane.");
