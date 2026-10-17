using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Policies;

/// <summary>
/// Checks whether the parts of a robot fit together (R2–R4). Returns every violation instead of
/// throwing on the first, so the customer sees all problems at once. A rule is checked only when the
/// parts it compares are present exactly once; a missing or duplicated part is R1's problem.
/// </summary>
public static class RobotPartsCompatibilityPolicy
{
    public static IReadOnlyList<RuleViolation> FindViolations(IReadOnlyCollection<PartSnapshot> parts)
    {
        var housing = SinglePartOf(parts, PartCategoryEnum.Housing);
        var drive = SinglePartOf(parts, PartCategoryEnum.Drive);
        var electronics = SinglePartOf(parts, PartCategoryEnum.Electronics);

        List<RuleViolation> violations = [];

        if (housing is not null && drive is not null && housing.MountType != drive.MountType)
        {
            violations.Add(new RuleViolation(
                "R2", $"Napęd „{drive.Name}” (mocowanie {drive.MountType}) nie pasuje do mocowania obudowy „{housing.Name}” ({housing.MountType})."));
        }

        if (drive is not null && electronics is not null && drive.VoltageVolts != electronics.VoltageVolts)
        {
            violations.Add(new RuleViolation(
                "R3", $"Napięcie elektroniki „{electronics.Name}” ({electronics.VoltageVolts} V) nie pasuje do napięcia napędu „{drive.Name}” ({drive.VoltageVolts} V)."));
        }

        var totalWeightGrams = parts.Sum(part => part.WeightGrams);

        if (drive?.MaxLoadGrams is { } maxLoadGrams && totalWeightGrams > maxLoadGrams)
        {
            violations.Add(new RuleViolation(
                "R4", $"Łączna masa części ({totalWeightGrams} g) przekracza udźwig napędu „{drive.Name}” ({maxLoadGrams} g)."));
        }

        return violations;
    }

    private static PartSnapshot? SinglePartOf(IReadOnlyCollection<PartSnapshot> parts, PartCategoryEnum category)
    {
        var partsOfCategory = parts.Where(part => part.Category == category).ToList();

        return partsOfCategory.Count == 1 ? partsOfCategory[0] : null;
    }
}
