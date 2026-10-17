using RoboForge.Modules.Production.Domain.Jobs.Enums;

namespace RoboForge.Modules.Production.Domain.Jobs.ValueObjects;

/// <summary>Who completed a stage, and when.</summary>
public sealed record StageCompletion(ProductionStageEnum Stage, string CompletedBy, DateTime CompletedOnUtc);
