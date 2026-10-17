namespace RoboForge.Modules.Catalog.Application.Parts.Resources;

/// <summary>A list of catalog parts as the API returns it.</summary>
public sealed record CollectionPartResource(IReadOnlyList<PartResource> Items);
