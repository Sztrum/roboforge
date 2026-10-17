using Mediator;
using RoboForge.Modules.Catalog.Application.Parts.Resources;

namespace RoboForge.Modules.Catalog.Application.Parts.Queries;

/// <summary>Shows one catalog part.</summary>
public sealed record ShowPartQuery(Guid PartId) : IQuery<PartResource>;
