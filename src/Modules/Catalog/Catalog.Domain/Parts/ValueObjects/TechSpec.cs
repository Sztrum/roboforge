using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Catalog.Domain.Parts.Exceptions;

namespace RoboForge.Modules.Catalog.Domain.Parts.ValueObjects;

/// <summary>
/// The technical values of a part that compatibility rules R2–R4 compare. Every part has a weight;
/// the other values apply only to some categories, so they are optional here and required per
/// category by <see cref="PartAggregate"/> (rule C3).
/// </summary>
public sealed record TechSpec
{
    public TechSpec(int weightGrams, MountTypeEnum? mountType = null, int? voltageVolts = null, int? maxLoadGrams = null)
    {
        GuardPositive(nameof(WeightGrams), weightGrams);

        if (voltageVolts is { } voltage)
        {
            GuardPositive(nameof(VoltageVolts), voltage);
        }

        if (maxLoadGrams is { } maxLoad)
        {
            GuardPositive(nameof(MaxLoadGrams), maxLoad);
        }

        WeightGrams = weightGrams;
        MountType = mountType;
        VoltageVolts = voltageVolts;
        MaxLoadGrams = maxLoadGrams;
    }

    public int WeightGrams { get; }

    public MountTypeEnum? MountType { get; }

    public int? VoltageVolts { get; }

    public int? MaxLoadGrams { get; }

    private static void GuardPositive(string valueName, int value)
    {
        if (value <= 0)
        {
            throw new TechSpecValueMustBePositiveException(valueName, value);
        }
    }
}
