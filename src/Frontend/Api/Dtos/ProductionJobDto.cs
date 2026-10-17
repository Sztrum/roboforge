namespace RoboForge.Frontend.Api.Dtos;

public sealed record ProductionJobDto(
    Guid Id,
    Guid OrderId,
    string CurrentStage,
    string Status,
    DateTime StartedOnUtc,
    IReadOnlyList<ProductionPartDto> Parts,
    IReadOnlyList<StageCompletionDto> CompletedStages,
    IReadOnlyList<QaFailureDto> QaFailures,
    string? TrackingNumber);
