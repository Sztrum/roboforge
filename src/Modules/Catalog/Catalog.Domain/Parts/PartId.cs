namespace RoboForge.Modules.Catalog.Domain.Parts;

/// <summary>Identifies a catalog part; a separate type so it cannot be mixed up with other IDs.</summary>
public readonly record struct PartId(Guid Value)
{
    public static PartId New() => new(Guid.CreateVersion7());
}
