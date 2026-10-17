using RoboForge.Shared.Domain.Events;

namespace RoboForge.Modules.Sales.Domain.Orders.Events;

public sealed record OrderCancelledEvent(OrderId OrderId, string Reason) : IDomainEvent;
