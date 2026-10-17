using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Catalog.Domain.Parts.Exceptions;

/// <summary>Thrown when a part is created without a name (rule C1).</summary>
public sealed class PartNameCannotBeEmptyException()
    : DomainException("C1", "Nazwa części nie może być pusta.");
