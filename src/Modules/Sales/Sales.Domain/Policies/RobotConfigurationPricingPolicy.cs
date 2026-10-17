using RoboForge.Modules.Sales.Domain.Configurations.ValueObjects;
using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Modules.Sales.Domain.Policies;

/// <summary>
/// Prices a robot (R5): the sum of part prices plus 15% for assembly; robots from 1000 PLN
/// (parts and assembly together) get 5% off.
/// </summary>
public static class RobotConfigurationPricingPolicy
{
    public const decimal AssemblyRate = 0.15m;

    public const decimal DiscountRate = 0.05m;

    public const decimal DiscountThresholdAmount = 1000m;

    public static RobotPrice CalculatePrice(IReadOnlyCollection<PartSnapshot> parts)
    {
        var partsPrice = parts.Aggregate(Money.Zero(Money.DefaultCurrency), (sum, part) => sum + part.Price);
        var assemblyPrice = partsPrice * AssemblyRate;
        var priceBeforeDiscount = partsPrice + assemblyPrice;

        var discount = priceBeforeDiscount.Amount >= DiscountThresholdAmount
            ? priceBeforeDiscount * DiscountRate
            : Money.Zero(priceBeforeDiscount.Currency);

        return new RobotPrice(partsPrice, assemblyPrice, discount, priceBeforeDiscount - discount);
    }
}
