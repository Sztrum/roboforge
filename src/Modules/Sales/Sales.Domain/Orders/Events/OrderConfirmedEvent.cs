using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.Events;

namespace RoboForge.Modules.Sales.Domain.Orders.Events;

public sealed record OrderConfirmedEvent(OrderId OrderId, IReadOnlyList<PartSnapshot> Parts) : IDomainEvent;
