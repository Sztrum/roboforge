using Mediator;
using RoboForge.Modules.Production.Application.Jobs.Resources;

namespace RoboForge.Modules.Production.Application.Jobs.Commands;

/// <summary>Records a failed quality check (R10).</summary>
public sealed record FailProductionJobQaCommand(Guid JobId, string Reason) : ICommand<ProductionJobResource>;
