namespace RoboForge.Frontend.Api.Dtos;

public sealed record PartDto(
    Guid Id,
    string Name,
    string Category,
    decimal PriceAmount,
    string PriceCurrency,
    int LeadTimeWorkingDays,
    int WeightGrams,
    string? MountType,
    int? VoltageVolts,
    int? MaxLoadGrams,
    bool IsDiscontinued);
