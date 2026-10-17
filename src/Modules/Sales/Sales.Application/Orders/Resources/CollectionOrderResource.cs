namespace RoboForge.Modules.Sales.Application.Orders.Resources;

public sealed record CollectionOrderResource(IReadOnlyList<OrderResource> Items);
