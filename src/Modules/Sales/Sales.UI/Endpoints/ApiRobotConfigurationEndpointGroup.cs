using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RoboForge.Modules.Sales.Application.Configurations.Commands;
using RoboForge.Modules.Sales.Application.Configurations.Queries;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.UI.Endpoints.Dtos;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Sales.UI.Endpoints;

/// <summary>Robot configurations customers build from catalog parts.</summary>
public sealed class ApiRobotConfigurationEndpointGroup : IEndpointGroup
{
    public static string? RoutePrefix => "/api/configurations";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(StoreRobotConfiguration);
        groupBuilder.MapGet(ShowRobotConfiguration, "{configurationId:guid}");
        groupBuilder.MapPost(AddPartToRobotConfiguration, "{configurationId:guid}/parts");
        groupBuilder.MapDelete(RemovePartFromRobotConfiguration, "{configurationId:guid}/parts/{partId:guid}");
        groupBuilder.MapGet(ShowRobotConfigurationQuote, "{configurationId:guid}/quote");
    }

    /// <summary>Starts a new, empty robot configuration.</summary>
    public static async Task<Created<RobotConfigurationResource>> StoreRobotConfiguration(ISender sender, CancellationToken cancellationToken)
    {
        var configuration = await sender.Send(new StoreRobotConfigurationCommand(), cancellationToken);

        return TypedResults.Created($"{RoutePrefix}/{configuration.Id}", configuration);
    }

    /// <summary>Shows one robot configuration with its parts.</summary>
    public static async Task<Ok<RobotConfigurationResource>> ShowRobotConfiguration(
        ISender sender,
        Guid configurationId,
        CancellationToken cancellationToken)
    {
        var configuration = await sender.Send(new ShowRobotConfigurationQuery(configurationId), cancellationToken);

        return TypedResults.Ok(configuration);
    }

    /// <summary>Adds a snapshot of an offered part to a draft configuration.</summary>
    public static async Task<Ok<RobotConfigurationResource>> AddPartToRobotConfiguration(
        ISender sender,
        Guid configurationId,
        AddPartToRobotConfigurationRequestDto request,
        CancellationToken cancellationToken)
    {
        var configuration = await sender.Send(new AddPartToRobotConfigurationCommand(configurationId, request.PartId), cancellationToken);

        return TypedResults.Ok(configuration);
    }

    /// <summary>Removes one piece of a part from a draft configuration.</summary>
    public static async Task<Ok<RobotConfigurationResource>> RemovePartFromRobotConfiguration(
        ISender sender,
        Guid configurationId,
        Guid partId,
        CancellationToken cancellationToken)
    {
        var configuration = await sender.Send(new RemovePartFromRobotConfigurationCommand(configurationId, partId), cancellationToken);

        return TypedResults.Ok(configuration);
    }

    /// <summary>Validates a configuration (R1–R4, R7) and computes its price (R5) and lead time (R6).</summary>
    public static async Task<Ok<RobotQuoteResource>> ShowRobotConfigurationQuote(
        ISender sender,
        Guid configurationId,
        CancellationToken cancellationToken)
    {
        var quote = await sender.Send(new ShowRobotConfigurationQuoteQuery(configurationId), cancellationToken);

        return TypedResults.Ok(quote);
    }
}
