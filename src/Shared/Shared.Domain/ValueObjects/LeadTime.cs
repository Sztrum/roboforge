using RoboForge.Shared.Domain.Exceptions;

namespace RoboForge.Shared.Domain.ValueObjects;

/// <summary>How many working days it takes to deliver a part or build a robot.</summary>
public sealed record LeadTime
{
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
}
