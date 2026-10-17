using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Modules.Catalog.Domain.Parts.Exceptions;

/// <summary>Thrown when a part lacks a technical value its category needs for compatibility checks (rule C3).</summary>
public sealed class PartTechSpecIsMissingValueForCategoryException(PartCategoryEnum category, string missingValueName)
    : DomainException("C3", $"Część z kategorii {category} musi mieć podaną wartość {missingValueName}.");
