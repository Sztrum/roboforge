using RoboForge.Shared.Domain.ValueObjects;

namespace RoboForge.Shared.Domain.UnitTests.ValueObjects;

public class LeadTimeCombinationTest
{
    [Fact]
    public void Combine_PartLeadTimes_TakesLongestPlusAssemblyAndQa()
    {
        LeadTime[] partLeadTimes = [new(3), new(8), new(1)];

        var robotLeadTime = LeadTime.Combine(partLeadTimes);

        robotLeadTime.ShouldBe(new LeadTime(8 + 2 + 1));
    }

    [Fact]
    public void Combine_WithoutParts_IsAssemblyAndQaOnly()
    {
        LeadTime.Combine([]).ShouldBe(new LeadTime(3));
    }

    [Fact]
    public void ExtendBy_ExtraDays_AddsThem()
    {
        new LeadTime(8).ExtendBy(3).ShouldBe(new LeadTime(11));
    }
}
