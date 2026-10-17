using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Production.Domain.Jobs.Exceptions;

/// <summary>Thrown when a job that is not on hold is released from hold (rule P3).</summary>
public sealed class ProductionJobIsNotOnHoldException(ProductionJobId jobId)
    : DomainException("P3", $"Zlecenie {jobId.Value} nie jest wstrzymane.");
