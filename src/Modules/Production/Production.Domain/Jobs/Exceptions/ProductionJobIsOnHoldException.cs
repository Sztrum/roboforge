using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Production.Domain.Jobs.Exceptions;

/// <summary>Thrown when work continues on a job held after two failed QA checks, before a decision (rule R10).</summary>
public sealed class ProductionJobIsOnHoldException(ProductionJobId jobId)
    : DomainException("R10", $"Zlecenie {jobId.Value} jest wstrzymane po dwóch nieudanych kontrolach jakości i czeka na decyzję.");
