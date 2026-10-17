namespace RoboForge.Modules.Production.Application.Jobs.Resources;

public sealed record QaFailureResource(string Reason, DateTime FailedOnUtc);
