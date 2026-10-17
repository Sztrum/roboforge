using RoboForge.Modules.Sales.Domain.Configurations.ValueObjects;

namespace RoboForge.Modules.Sales.Application.Configurations.Resources;

/// <summary>Validation, price breakdown (R5) and lead time (R6) of a configuration.</summary>
public sealed record RobotQuoteResource(
    bool IsValid,
    IReadOnlyList<RuleViolationResource> Violations,
    decimal PartsPriceAmount,
    decimal AssemblyPriceAmount,
    decimal DiscountAmount,
    decimal TotalPriceAmount,
    string Currency,
    int LeadTimeWorkingDays)
{
    public static RobotQuoteResource FromQuote(RobotQuote quote) => new(
        quote.IsValid,
        [.. quote.Violations.Select(RuleViolationResource.FromViolation)],
        quote.Price.PartsPrice.Amount,
        quote.Price.AssemblyPrice.Amount,
        quote.Price.Discount.Amount,
        quote.Price.TotalPrice.Amount,
        quote.Price.TotalPrice.Currency,
        quote.LeadTime.WorkingDays);
}
