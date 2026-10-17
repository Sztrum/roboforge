using RoboForge.Modules.Sales.Domain.Configurations;
using RoboForge.Modules.Sales.Domain.Configurations.Enums;
using RoboForge.Modules.Sales.Domain.Configurations.Exceptions;
using RoboForge.Shared.Domain.ValueObjects;
using static RoboForge.Modules.Sales.Domain.UnitTests.Parts.PartSnapshotExamples;

namespace RoboForge.Modules.Sales.Domain.UnitTests.Configurations;

public class RobotConfigurationAggregateOrderingTest
{
    private static RobotConfigurationAggregate CompatibleConfiguration()
    {
        var configuration = RobotConfigurationAggregate.Create();
        foreach (var part in CompatibleRangerRobot())
        {
            configuration.AddPart(part);
        }

        return configuration;
    }

    [Fact]
    public void PrepareQuote_CompatibleRobot_IsValidWithPriceAndLeadTime()
    {
        var quote = CompatibleConfiguration().PrepareQuote();

        quote.IsValid.ShouldBeTrue();
        quote.Price.TotalPrice.ShouldBe(new Money(788.90m, "PLN"));
        quote.LeadTime.ShouldBe(new LeadTime(8));
    }

    [Fact]
    public void PrepareQuote_IncompleteRobot_ListsViolations()
    {
        var configuration = RobotConfigurationAggregate.Create();
        configuration.AddPart(RangerHousing());

        var quote = configuration.PrepareQuote();

        quote.IsValid.ShouldBeFalse();
        quote.Violations.Count.ShouldBe(2);
    }

    [Fact]
    public void MarkAsOrdered_CompatibleRobot_LocksConfiguration()
    {
        var configuration = CompatibleConfiguration();

        configuration.MarkAsOrdered();

        configuration.Status.ShouldBe(RobotConfigurationStatusEnum.Ordered);
    }

    [Fact]
    public void MarkAsOrdered_InvalidRobot_ThrowsRobotConfigurationIsNotValidExceptionWithEveryViolation()
    {
        var configuration = RobotConfigurationAggregate.Create();
        configuration.AddPart(TitanHousing());
        configuration.AddPart(MicroDrive());
        configuration.AddPart(NanoElectronics());

        var exception = Should.Throw<RobotConfigurationIsNotValidException>(configuration.MarkAsOrdered);

        exception.Violations.Select(violation => violation.Rule).ShouldBe(["R2", "R4"]);
    }

    [Fact]
    public void AddPart_ToOrderedConfiguration_ThrowsOrderedRobotConfigurationCannotBeChangedException()
    {
        var configuration = CompatibleConfiguration();
        configuration.MarkAsOrdered();

        var exception = Should.Throw<OrderedRobotConfigurationCannotBeChangedException>(() => configuration.AddPart(Camera()));

        exception.Rule.ShouldBe("S1");
    }

    [Fact]
    public void MarkAsOrdered_Twice_ThrowsOrderedRobotConfigurationCannotBeChangedException()
    {
        var configuration = CompatibleConfiguration();
        configuration.MarkAsOrdered();

        Should.Throw<OrderedRobotConfigurationCannotBeChangedException>(configuration.MarkAsOrdered);
    }
}
