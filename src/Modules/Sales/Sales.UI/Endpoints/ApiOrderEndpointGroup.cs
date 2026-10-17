using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RoboForge.Modules.Sales.Application.Orders.Commands;
using RoboForge.Modules.Sales.Application.Orders.Queries;
using RoboForge.Modules.Sales.Application.Orders.Resources;
using RoboForge.Modules.Sales.UI.Endpoints.Dtos;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Sales.UI.Endpoints;

/// <summary>Customer orders, from placing to shipping.</summary>
public sealed class ApiOrderEndpointGroup : IEndpointGroup
{
    public static string? RoutePrefix => "/api/orders";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(IndexOrders);
        groupBuilder.MapGet(ShowOrder, "{orderId:guid}");
        groupBuilder.MapPost(PlaceOrder);
        groupBuilder.MapPost(ConfirmOrder, "{orderId:guid}/confirm");
        groupBuilder.MapPost(CancelOrder, "{orderId:guid}/cancel");
    }

    /// <summary>Lists orders, newest first.</summary>
    public static async Task<Ok<CollectionOrderResource>> IndexOrders(ISender sender, CancellationToken cancellationToken)
    {
        var orders = await sender.Send(new IndexOrderQuery(), cancellationToken);

        return TypedResults.Ok(orders);
    }

    /// <summary>Shows an order with its production history.</summary>
    public static async Task<Ok<OrderResource>> ShowOrder(ISender sender, Guid orderId, CancellationToken cancellationToken)
    {
        var order = await sender.Send(new ShowOrderQuery(orderId), cancellationToken);

        return TypedResults.Ok(order);
    }

    /// <summary>Places an order from a valid robot configuration (all violated rules are returned at once).</summary>
    public static async Task<Created<OrderResource>> PlaceOrder(ISender sender, PlaceOrderCommand command, CancellationToken cancellationToken)
    {
        var order = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"{RoutePrefix}/{order.Id}", order);
    }

    /// <summary>Confirms a placed order, which starts production.</summary>
    public static async Task<Ok<OrderResource>> ConfirmOrder(ISender sender, Guid orderId, CancellationToken cancellationToken)
    {
        var order = await sender.Send(new ConfirmOrderCommand(orderId), cancellationToken);

        return TypedResults.Ok(order);
    }

    /// <summary>Cancels an order before production starts (R8).</summary>
    public static async Task<Ok<OrderResource>> CancelOrder(
        ISender sender,
        Guid orderId,
        CancelOrderRequestDto request,
        CancellationToken cancellationToken)
    {
        var order = await sender.Send(new CancelOrderCommand(orderId, request.Reason), cancellationToken);

        return TypedResults.Ok(order);
    }
}
