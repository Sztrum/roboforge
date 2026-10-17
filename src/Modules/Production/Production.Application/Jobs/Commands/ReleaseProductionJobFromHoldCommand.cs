using Mediator;
using RoboForge.Modules.Production.Application.Jobs.Resources;

namespace RoboForge.Modules.Production.Application.Jobs.Commands;

/// <summary>Lets work on a held job continue from assembly.</summary>
public sealed record ReleaseProductionJobFromHoldCommand(Guid JobId) : ICommand<ProductionJobResource>;
