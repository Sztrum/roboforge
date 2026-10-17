using Mediator;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;

namespace RoboForge.Modules.Catalog.Application.Parts.Queries;

/// <summary>Lists catalog parts, optionally only one category.</summary>
public sealed record IndexPartQuery(PartCategoryEnum? Category) : IQuery<CollectionPartResource>;
