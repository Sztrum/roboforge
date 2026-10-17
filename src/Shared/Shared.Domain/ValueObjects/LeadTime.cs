using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Shared.Domain.ValueObjects;

/// <summary>How many working days it takes to deliver a part or build a robot.</summary>
public sealed record LeadTime
{
    public const int AssemblyWorkingDays = 2;

    public const int QaWorkingDays = 1;

    private const int MinimumWorkingDays = 1;

    public LeadTime(int workingDays)
    {
        if (workingDays < MinimumWorkingDays)
        {
            throw new LeadTimeMustBeAtLeastOneWorkingDayException(workingDays);
        }

        WorkingDays = workingDays;
    }

    public int WorkingDays { get; }

    /// <summary>
    /// The lead time of a robot built from parts (R6): the longest part lead time, because parts are
    /// ordered in parallel, plus assembly and QA.
    /// </summary>
    public static LeadTime Combine(IEnumerable<LeadTime> partLeadTimes)
    {
        var longestPartWorkingDays = partLeadTimes.Select(leadTime => leadTime.WorkingDays).DefaultIfEmpty(0).Max();

        return new LeadTime(longestPartWorkingDays + AssemblyWorkingDays + QaWorkingDays);
    }

    /// <summary>This lead time made longer by the given working days, e.g. after a failed QA.</summary>
    public LeadTime ExtendBy(int extraWorkingDays) => new(WorkingDays + extraWorkingDays);
}
