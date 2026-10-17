namespace RoboForge.Frontend.Api.Dtos;

public sealed record OrderDto(
    Guid Id,
    Guid ConfigurationId,
    string Status,
    IReadOnlyList<ConfigurationPartDto> Parts,
    decimal PartsPriceAmount,
    decimal AssemblyPriceAmount,
    decimal DiscountAmount,
    decimal TotalPriceAmount,
    string Currency,
    int LeadTimeWorkingDays,
    DateTime PlacedOnUtc,
    IReadOnlyList<OrderStageDto> CompletedStages,
    IReadOnlyList<OrderNoteDto> Notes,
    string? TrackingNumber,
    string? CancellationReason);
