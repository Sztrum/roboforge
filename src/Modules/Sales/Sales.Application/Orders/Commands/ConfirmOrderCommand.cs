using Mediator;
using RoboForge.Modules.Sales.Application.Orders.Resources;

namespace RoboForge.Modules.Sales.Application.Orders.Commands;

/// <summary>Confirms a placed order, which starts production.</summary>
public sealed record ConfirmOrderCommand(Guid OrderId) : ICommand<OrderResource>;
