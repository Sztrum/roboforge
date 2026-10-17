using Mediator;
using RoboForge.Modules.Sales.Application.Orders.Resources;

namespace RoboForge.Modules.Sales.Application.Orders.Commands;

/// <summary>Places an order from a valid robot configuration.</summary>
public sealed record PlaceOrderCommand(Guid ConfigurationId) : ICommand<OrderResource>;
