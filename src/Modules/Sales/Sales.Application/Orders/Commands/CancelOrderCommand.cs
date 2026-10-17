using Mediator;
using RoboForge.Modules.Sales.Application.Orders.Resources;

namespace RoboForge.Modules.Sales.Application.Orders.Commands;

/// <summary>Cancels an order before production starts (R8).</summary>
public sealed record CancelOrderCommand(Guid OrderId, string Reason) : ICommand<OrderResource>;
