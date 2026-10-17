namespace RoboForge.Frontend.Api.Dtos;

public sealed record StageCompletionDto(string Stage, string CompletedBy, DateTime CompletedOnUtc);
