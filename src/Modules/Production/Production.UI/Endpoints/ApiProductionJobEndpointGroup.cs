using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using RoboForge.Modules.Production.Application.Jobs.Commands;
using RoboForge.Modules.Production.Application.Jobs.Queries;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Modules.Production.UI.Endpoints.Dtos;
using RoboForge.Shared.UI.Endpoints;

namespace RoboForge.Modules.Production.UI.Endpoints;

/// <summary>The workshop: production jobs moving through the stages.</summary>
public sealed class ApiProductionJobEndpointGroup : IEndpointGroup
{
    public static string? RoutePrefix => "/api/production/jobs";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapGet(IndexProductionJobs);
        groupBuilder.MapGet(ShowProductionJob, "{jobId:guid}");
        groupBuilder.MapPost(CompleteProductionStage, "{jobId:guid}/complete-stage");
        groupBuilder.MapPost(FailProductionJobQa, "{jobId:guid}/fail-qa");
        groupBuilder.MapPost(ReleaseProductionJobFromHold, "{jobId:guid}/release-hold");
        groupBuilder.MapPost(ShipProductionJob, "{jobId:guid}/ship");
    }

    /// <summary>The workshop queue, oldest first, optionally only jobs at one stage.</summary>
    public static async Task<Ok<CollectionProductionJobResource>> IndexProductionJobs(
        ISender sender,
        ProductionStageEnum? stage,
        CancellationToken cancellationToken)
    {
        var jobs = await sender.Send(new IndexProductionJobQuery(stage), cancellationToken);

        return TypedResults.Ok(jobs);
    }

    /// <summary>Shows one workshop job with its history.</summary>
    public static async Task<Ok<ProductionJobResource>> ShowProductionJob(ISender sender, Guid jobId, CancellationToken cancellationToken)
    {
        var job = await sender.Send(new ShowProductionJobQuery(jobId), cancellationToken);

        return TypedResults.Ok(job);
    }

    /// <summary>Completes the job's current stage; stages run strictly in order (R9).</summary>
    public static async Task<Ok<ProductionJobResource>> CompleteProductionStage(
        ISender sender,
        Guid jobId,
        CompleteProductionStageRequestDto request,
        CancellationToken cancellationToken)
    {
        var job = await sender.Send(new CompleteProductionStageCommand(jobId, request.Stage, request.CompletedBy), cancellationToken);

        return TypedResults.Ok(job);
    }

    /// <summary>Records a failed quality check: back to assembly, on hold after the second failure (R10).</summary>
    public static async Task<Ok<ProductionJobResource>> FailProductionJobQa(
        ISender sender,
        Guid jobId,
        FailProductionJobQaRequestDto request,
        CancellationToken cancellationToken)
    {
        var job = await sender.Send(new FailProductionJobQaCommand(jobId, request.Reason), cancellationToken);

        return TypedResults.Ok(job);
    }

    /// <summary>Lets work on a held job continue from assembly.</summary>
    public static async Task<Ok<ProductionJobResource>> ReleaseProductionJobFromHold(ISender sender, Guid jobId, CancellationToken cancellationToken)
    {
        var job = await sender.Send(new ReleaseProductionJobFromHoldCommand(jobId), cancellationToken);

        return TypedResults.Ok(job);
    }

    /// <summary>Ships the packed robot with a tracking number.</summary>
    public static async Task<Ok<ProductionJobResource>> ShipProductionJob(
        ISender sender,
        Guid jobId,
        ShipProductionJobRequestDto request,
        CancellationToken cancellationToken)
    {
        var job = await sender.Send(new ShipProductionJobCommand(jobId, request.TrackingNumber), cancellationToken);

        return TypedResults.Ok(job);
    }
}
