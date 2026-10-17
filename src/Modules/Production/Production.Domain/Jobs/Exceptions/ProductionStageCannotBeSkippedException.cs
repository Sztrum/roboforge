using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Production.Domain.Jobs.Exceptions;

/// <summary>Thrown when a stage other than the current one is completed (rule R9).</summary>
public sealed class ProductionStageCannotBeSkippedException(ProductionStageEnum currentStage, ProductionStageEnum attemptedStage)
    : DomainException("R9", $"Etapy idą po kolei: teraz trwa {currentStage}, nie można zakończyć {attemptedStage}.");
