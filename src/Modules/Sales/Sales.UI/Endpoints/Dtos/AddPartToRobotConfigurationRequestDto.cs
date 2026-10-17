namespace RoboForge.Modules.Sales.UI.Endpoints.Dtos;

/// <summary>The body of <c>POST /api/configurations/{id}/parts</c>: which catalog part to add.</summary>
public sealed record AddPartToRobotConfigurationRequestDto(Guid PartId);
