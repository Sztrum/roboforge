using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.Exceptions;

/// <summary>Thrown when a <see cref="LeadTime"/> is shorter than one working day (rule L1).</summary>
public sealed class LeadTimeMustBeAtLeastOneWorkingDayException(int workingDays)
    : DomainException("L1", $"Czas realizacji musi wynosić co najmniej 1 dzień roboczy (podano {workingDays}).");
