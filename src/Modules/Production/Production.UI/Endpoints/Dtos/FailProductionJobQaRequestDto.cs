namespace RoboForge.Modules.Production.UI.Endpoints.Dtos;

/// <summary>The body of <c>POST /api/production/jobs/{id}/fail-qa</c>.</summary>
public sealed record FailProductionJobQaRequestDto(string Reason);
