namespace RoboForge.Frontend.Api.Dtos;

public sealed record ConfigurationDto(Guid Id, string Status, IReadOnlyList<ConfigurationPartDto> Parts);
