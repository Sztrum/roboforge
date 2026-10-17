using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Catalog.Domain.Parts.Exceptions;

/// <summary>Thrown when the price of a discontinued part is changed (rule C4).</summary>
public sealed class DiscontinuedPartCannotBeRepricedException(PartId partId)
    : DomainException("C4", $"Nie można zmienić ceny wycofanej części {partId.Value}.");
