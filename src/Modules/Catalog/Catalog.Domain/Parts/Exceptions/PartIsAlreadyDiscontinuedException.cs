using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Catalog.Domain.Parts.Exceptions;

/// <summary>Thrown when an already discontinued part is discontinued again (rule C5).</summary>
public sealed class PartIsAlreadyDiscontinuedException(PartId partId)
    : DomainException("C5", $"Część {partId.Value} jest już wycofana z oferty.");
