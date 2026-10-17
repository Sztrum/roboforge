namespace RoboForge.Modules.Sales.IntegrationEvents;

/// <summary>A part of a confirmed order, as much as the workshop needs to build it. The category travels as its name.</summary>
public sealed record ConfirmedOrderPartDto(Guid PartId, string Name, string Category);
