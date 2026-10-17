using RoboForge.Modules.Production.Domain.Jobs.Enums;

namespace RoboForge.Modules.Production.Application.Jobs.Resources;

public sealed record StageCompletionResource(ProductionStageEnum Stage, string CompletedBy, DateTime CompletedOnUtc);
