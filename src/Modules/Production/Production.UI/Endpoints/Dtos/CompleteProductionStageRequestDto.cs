using RoboForge.Modules.Production.Domain.Jobs.Enums;

namespace RoboForge.Modules.Production.UI.Endpoints.Dtos;

/// <summary>The body of <c>POST /api/production/jobs/{id}/complete-stage</c>.</summary>
public sealed record CompleteProductionStageRequestDto(ProductionStageEnum Stage, string CompletedBy);
