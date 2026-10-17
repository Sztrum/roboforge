using RoboForge.Modules.Sales.Domain.Configurations.Enums;
using RoboForge.Modules.Sales.Domain.Configurations.Exceptions;
using RoboForge.Modules.Sales.Domain.Configurations.ValueObjects;
using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Modules.Sales.Domain.Parts.Enums;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Modules.Sales.Domain.Policies;
using RoboForge.Shared.Domain.Aggregates;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Configurations;

/// <summary>
/// A robot a customer is putting together from snapshots of catalog parts. It may be incomplete or
/// incompatible while being built; <see cref="Validate"/> lists what is still wrong, and an order can be
/// placed only from a valid configuration.
/// </summary>
public sealed class RobotConfigurationAggregate : AggregateRoot<RobotConfigurationId>
{
    public const int MaxSensors = 3;

    private readonly List<PartSnapshot> _parts = [];

    private RobotConfigurationAggregate(RobotConfigurationId id)
        : base(id)
    {
    }

    public RobotConfigurationStatusEnum Status { get; private set; }

    public IReadOnlyList<PartSnapshot> Parts => _parts.AsReadOnly();

    public static RobotConfigurationAggregate Create() => new(RobotConfigurationId.New());

    public void AddPart(PartSnapshot part)
    {
        GuardIsDraft();

        if (part.IsDiscontinued)
        {
            throw new DiscontinuedPartCannotBeAddedException(part.Name);
        }

        _parts.Add(part);
    }

    /// <summary>Removes one piece of the part; a configuration may contain the same sensor more than once.</summary>
    public void RemovePart(CatalogPartId partId)
    {
        GuardIsDraft();

        var partToRemove = _parts.Find(part => part.PartId == partId)
            ?? throw new PartIsNotInRobotConfigurationException(partId);

        _parts.Remove(partToRemove);
    }

    /// <summary>Records that the catalog withdrew a part this draft contains, so validation reports it (R7).</summary>
    public void MarkPartDiscontinued(CatalogPartId partId)
    {
        if (Status != RobotConfigurationStatusEnum.Draft)
        {
            return;
        }

        for (var index = 0; index < _parts.Count; index++)
        {
            if (_parts[index].PartId == partId)
            {
                _parts[index] = _parts[index].AsDiscontinued();
            }
        }
    }

    /// <summary>Every rule the configuration still breaks: its make-up (R1), discontinued parts (R7) and compatibility (R2–R4).</summary>
    public IReadOnlyList<RuleViolation> Validate() =>
    [
        .. FindCompositionViolations(),
        .. _parts.Where(part => part.IsDiscontinued).Select(part =>
            new RuleViolation("R7", $"Część „{part.Name}” została wycofana z oferty.")),
        .. RobotPartsCompatibilityPolicy.FindViolations(_parts),
    ];

    public RobotQuote PrepareQuote() =>
        new(
            Validate(),
            RobotConfigurationPricingPolicy.CalculatePrice(_parts),
            LeadTime.Combine(_parts.Select(part => part.LeadTime)));

    /// <summary>Locks a valid configuration because an order is being placed from it.</summary>
    public void MarkAsOrdered()
    {
        GuardIsDraft();

        var violations = Validate();

        if (violations.Count != 0)
        {
            throw new RobotConfigurationIsNotValidException(violations);
        }

        Status = RobotConfigurationStatusEnum.Ordered;
    }

    private IEnumerable<RuleViolation> FindCompositionViolations()
    {
        var housingCount = CountParts(PartCategoryEnum.Housing);
        var driveCount = CountParts(PartCategoryEnum.Drive);
        var electronicsCount = CountParts(PartCategoryEnum.Electronics);
        var sensorCount = CountParts(PartCategoryEnum.Sensor);

        if (housingCount != 1)
        {
            yield return new RuleViolation("R1", $"Robot musi mieć dokładnie 1 obudowę (ma {housingCount}).");
        }

        if (driveCount != 1)
        {
            yield return new RuleViolation("R1", $"Robot musi mieć dokładnie 1 napęd (ma {driveCount}).");
        }

        if (electronicsCount != 1)
        {
            yield return new RuleViolation("R1", $"Robot musi mieć dokładnie 1 moduł elektroniki (ma {electronicsCount}).");
        }

        if (sensorCount > MaxSensors)
        {
            yield return new RuleViolation("R1", $"Robot może mieć najwyżej {MaxSensors} czujniki (ma {sensorCount}).");
        }
    }

    private int CountParts(PartCategoryEnum category) => _parts.Count(part => part.Category == category);

    private void GuardIsDraft()
    {
        if (Status != RobotConfigurationStatusEnum.Draft)
        {
            throw new OrderedRobotConfigurationCannotBeChangedException(Id);
        }
    }
}
