namespace RoboForge.Modules.Production.Domain.Jobs.ValueObjects;

/// <summary>Why a quality check failed, and when.</summary>
public sealed record QaFailure(string Reason, DateTime FailedOnUtc);
