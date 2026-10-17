using Mediator;
using RoboForge.Modules.Sales.Application.Orders.Resources;

namespace RoboForge.Modules.Sales.Application.Orders.Queries;

/// <summary>Shows an order with its production history.</summary>
public sealed record ShowOrderQuery(Guid OrderId) : IQuery<OrderResource>;
