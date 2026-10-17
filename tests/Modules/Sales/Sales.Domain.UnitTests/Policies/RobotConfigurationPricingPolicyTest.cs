using RoboForge.Modules.Sales.Domain.Parts.ValueObjects;
using RoboForge.Modules.Sales.Domain.Policies;
using RoboForge.Shared.Domain.ValueObjects;
using static RoboForge.Modules.Sales.Domain.UnitTests.Parts.PartSnapshotExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Policies;

public class RobotConfigurationPricingPolicyTest
{
    [Fact]
    public void CalculatePrice_BelowThreshold_AddsAssemblyWithoutDiscount()
    {
        var price = RobotConfigurationPricingPolicy.CalculatePrice(CompatibleRangerRobot());

        price.PartsPrice.ShouldBe(new Money(686m, "PLN"));
        price.AssemblyPrice.ShouldBe(new Money(102.90m, "PLN"));
        price.Discount.ShouldBe(Money.Zero("PLN"));
        price.TotalPrice.ShouldBe(new Money(788.90m, "PLN"));
    }

    [Fact]
    public void CalculatePrice_AboveThreshold_GivesFivePercentOff()
    {
        PartSnapshot[] titanRobotWithLidar = [TitanHousing(), CrawlerDrive(), EdgeAiElectronics(), Lidar()];

        var price = RobotConfigurationPricingPolicy.CalculatePrice(titanRobotWithLidar);

        price.PartsPrice.ShouldBe(new Money(1656m, "PLN"));
        price.AssemblyPrice.ShouldBe(new Money(248.40m, "PLN"));
        price.Discount.ShouldBe(new Money(95.22m, "PLN"));
        price.TotalPrice.ShouldBe(new Money(1809.18m, "PLN"));
    }

    [Theory]
    [InlineData(869.56, 0, 999.99)]
    [InlineData(869.57, 50.00, 950.01)]
    public void CalculatePrice_AroundThreshold_CountsAssemblyIntoIt(decimal partsAmount, decimal expectedDiscountAmount, decimal expectedTotalAmount)
    {
        PartSnapshot[] singlePart = [RangerHousing() with { Price = new Money(partsAmount, "PLN") }];

        var price = RobotConfigurationPricingPolicy.CalculatePrice(singlePart);

        price.Discount.ShouldBe(new Money(expectedDiscountAmount, "PLN"));
        price.TotalPrice.ShouldBe(new Money(expectedTotalAmount, "PLN"));
    }

    [Fact]
    public void CalculatePrice_WithoutParts_IsZero()
    {
        var price = RobotConfigurationPricingPolicy.CalculatePrice([]);

        price.TotalPrice.ShouldBe(Money.Zero("PLN"));
    }
}
