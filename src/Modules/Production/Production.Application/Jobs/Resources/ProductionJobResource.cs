using RoboForge.Modules.Production.Domain.Jobs;
using RoboForge.Modules.Production.Domain.Jobs.Enums;

namespace RoboForge.Modules.Production.Application.Jobs.Resources;

/// <summary>A workshop job with its current stage and history.</summary>
public sealed record ProductionJobResource(
    Guid Id,
    Guid OrderId,
    ProductionStageEnum CurrentStage,
    ProductionJobStatusEnum Status,
    DateTime StartedOnUtc,
    IReadOnlyList<ProductionPartResource> Parts,
    IReadOnlyList<StageCompletionResource> CompletedStages,
    IReadOnlyList<QaFailureResource> QaFailures,
    string? TrackingNumber)
{
    public static ProductionJobResource FromAggregate(ProductionJobAggregate job) => new(
        job.Id.Value,
        job.OrderId.Value,
        job.CurrentStage,
        job.Status,
        job.StartedOnUtc,
        [.. job.Parts.Select(part => new ProductionPartResource(part.PartId, part.Name, part.Category))],
        [.. job.CompletedStages.Select(stage => new StageCompletionResource(stage.Stage, stage.CompletedBy, stage.CompletedOnUtc))],
        [.. job.QaFailures.Select(failure => new QaFailureResource(failure.Reason, failure.FailedOnUtc))],
        job.TrackingNumber);
}
