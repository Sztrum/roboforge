using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Sales.Domain.Configurations.Exceptions;

/// <summary>Thrown when a part the catalog no longer offers is added to a configuration (rule R7).</summary>
public sealed class DiscontinuedPartCannotBeAddedException(string partName)
    : DomainException("R7", $"Część „{partName}” została wycofana z oferty i nie można jej dodać.");
