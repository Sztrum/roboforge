using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RoboForge.Modules.Catalog.Application.Parts.Queries;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Catalog.UI.Endpoints;

/// <summary>The catalog of parts customers can buy.</summary>
public sealed class ApiPartEndpointGroup : IEndpointGroup
{
    public static string? RoutePrefix => "/api/parts";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(IndexParts);
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
}
