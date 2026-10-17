using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Production.Domain.Jobs.Exceptions;

/// <summary>Thrown when a shipped job is changed (rule P4).</summary>
public sealed class ProductionJobIsAlreadyShippedException(ProductionJobId jobId)
    : DomainException("P4", $"Robot ze zlecenia {jobId.Value} został już wysłany.");
