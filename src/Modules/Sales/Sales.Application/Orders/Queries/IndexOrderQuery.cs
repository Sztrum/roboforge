using Mediator;
using RoboForge.Modules.Sales.Application.Orders.Resources;

namespace RoboForge.Modules.Sales.Application.Orders.Queries;

/// <summary>Lists orders, newest first.</summary>
public sealed record IndexOrderQuery : IQuery<CollectionOrderResource>;
