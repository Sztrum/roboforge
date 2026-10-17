namespace RoboForge.Frontend.Api.Dtos;

public sealed record QuoteDto(
    bool IsValid,
    IReadOnlyList<RuleViolationDto> Violations,
    decimal PartsPriceAmount,
    decimal AssemblyPriceAmount,
    decimal DiscountAmount,
    decimal TotalPriceAmount,
    string Currency,
    int LeadTimeWorkingDays);
