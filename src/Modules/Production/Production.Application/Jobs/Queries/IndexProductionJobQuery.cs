using Mediator;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs.Enums;

namespace RoboForge.Modules.Production.Application.Jobs.Queries;

/// <summary>The workshop queue: jobs oldest first, optionally only those at one stage.</summary>
public sealed record IndexProductionJobQuery(ProductionStageEnum? Stage) : IQuery<CollectionProductionJobResource>;
