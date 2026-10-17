namespace RoboForge.Modules.Production.Domain.Jobs.ValueObjects;

/// <summary>A part the workshop needs for the robot, as the confirmed order listed it.</summary>
public sealed record ProductionPart(Guid PartId, string Name, string Category);
