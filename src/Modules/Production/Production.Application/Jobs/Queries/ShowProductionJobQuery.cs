using Mediator;
using RoboForge.Modules.Production.Application.Jobs.Resources;

namespace RoboForge.Modules.Production.Application.Jobs.Queries;

/// <summary>Shows one workshop job with its history.</summary>
public sealed record ShowProductionJobQuery(Guid JobId) : IQuery<ProductionJobResource>;
