using Mediator;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs.Enums;

namespace RoboForge.Modules.Production.Application.Jobs.Commands;

/// <summary>Completes the job's current stage (R9).</summary>
public sealed record CompleteProductionStageCommand(Guid JobId, ProductionStageEnum Stage, string CompletedBy) : ICommand<ProductionJobResource>;
