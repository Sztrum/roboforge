namespace RoboForge.Frontend.Api.Dtos;

public sealed record QaFailureDto(string Reason, DateTime FailedOnUtc);
