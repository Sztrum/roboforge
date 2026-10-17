using RoboForge.Modules.Production.Domain.Jobs.Enums;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Production.Domain.Jobs.Exceptions;

/// <summary>Thrown when a failed QA is recorded while the job is not at the QA stage (rule P2).</summary>
public sealed class QaCanFailOnlyDuringQaTestsException(ProductionStageEnum currentStage)
    : DomainException("P2", $"Kontrola jakości może się nie udać tylko na etapie testów (teraz trwa {currentStage}).");
