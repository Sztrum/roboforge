namespace RoboForge.Modules.Sales.Domain.Parts;

/// <summary>The ID a part has in the catalog; Sales keeps it to know which part a copy or snapshot is of.</summary>
public readonly record struct CatalogPartId(Guid Value);
