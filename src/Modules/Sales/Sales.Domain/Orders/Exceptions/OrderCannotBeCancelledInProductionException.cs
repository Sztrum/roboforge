using RoboForge.Modules.Sales.Domain.Orders.Enums;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Sales.Domain.Orders.Exceptions;

/// <summary>Thrown when an order is cancelled after production started, i.e. after it was confirmed (rule R8).</summary>
public sealed class OrderCannotBeCancelledInProductionException(OrderId orderId, OrderStatusEnum status)
    : DomainException("R8", $"Zamówienia {orderId.Value} nie można anulować, bo produkcja już się rozpoczęła (status {status}).");
