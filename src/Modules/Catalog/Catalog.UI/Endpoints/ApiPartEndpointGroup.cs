using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RoboForge.Modules.Catalog.Application.Parts.Commands;
using RoboForge.Modules.Catalog.Application.Parts.Queries;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.UI.Endpoints.Dtos;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Catalog.UI.Endpoints;

/// <summary>The catalog of parts customers can buy.</summary>
public sealed class ApiPartEndpointGroup : IEndpointGroup
{
    public static string? RoutePrefix => "/api/parts";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(IndexParts);
        groupBuilder.MapGet(ShowPart, "{partId:guid}");
        groupBuilder.MapPost(StorePart);
        groupBuilder.MapPut(ChangePartPrice, "{partId:guid}/price");
        groupBuilder.MapPost(DiscontinuePart, "{partId:guid}/discontinue");
    }

    /// <summary>Lists catalog parts, optionally only one category.</summary>
    public static async Task<Ok<CollectionPartResource>> IndexParts(
        ISender sender,
        PartCategoryEnum? category,
        CancellationToken cancellationToken)
    {
        var parts = await sender.Send(new IndexPartQuery(category), cancellationToken);

        return TypedResults.Ok(parts);
    }

    /// <summary>Shows one catalog part.</summary>
    public static async Task<Ok<PartResource>> ShowPart(ISender sender, Guid partId, CancellationToken cancellationToken)
    {
        var part = await sender.Send(new ShowPartQuery(partId), cancellationToken);

        return TypedResults.Ok(part);
    }

    /// <summary>Adds a new part to the catalog; the price is in PLN.</summary>
    public static async Task<Created<PartResource>> StorePart(
        ISender sender,
        StorePartCommand command,
        CancellationToken cancellationToken)
    {
        var part = await sender.Send(command, cancellationToken);

        return TypedResults.Created($"{RoutePrefix}/{part.Id}", part);
    }

    /// <summary>Sets a new price (in PLN) for a part that is still offered.</summary>
    public static async Task<Ok<PartResource>> ChangePartPrice(
        ISender sender,
        Guid partId,
        ChangePartPriceRequestDto request,
        CancellationToken cancellationToken)
    {
        var part = await sender.Send(new ChangePartPriceCommand(partId, request.PriceAmount), cancellationToken);

        return TypedResults.Ok(part);
    }

    /// <summary>Withdraws a part from the offer for good.</summary>
    public static async Task<Ok<PartResource>> DiscontinuePart(ISender sender, Guid partId, CancellationToken cancellationToken)
    {
        var part = await sender.Send(new DiscontinuePartCommand(partId), cancellationToken);

        return TypedResults.Ok(part);
    }
}
