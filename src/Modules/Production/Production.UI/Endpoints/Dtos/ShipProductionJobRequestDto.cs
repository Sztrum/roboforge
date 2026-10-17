namespace RoboForge.Modules.Production.UI.Endpoints.Dtos;

/// <summary>The body of <c>POST /api/production/jobs/{id}/ship</c>.</summary>
public sealed record ShipProductionJobRequestDto(string TrackingNumber);
