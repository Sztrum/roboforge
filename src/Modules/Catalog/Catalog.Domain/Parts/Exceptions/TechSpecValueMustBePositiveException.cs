using RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Catalog.Domain.Parts.Exceptions;

/// <summary>Thrown when a <see cref="TechSpec"/> value such as weight or voltage is zero or negative (rule C2).</summary>
public sealed class TechSpecValueMustBePositiveException(string valueName, int value)
    : DomainException("C2", $"Wartość {valueName} musi być dodatnia (podano {value}).");
