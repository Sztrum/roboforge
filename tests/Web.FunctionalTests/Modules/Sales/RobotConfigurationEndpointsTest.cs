using System.Net;
using System.Net.Http.Json;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Domain.Configurations.Enums;

namespace RoboForge.Web.FunctionalTests.Modules.Sales;

public class RobotConfigurationEndpointsTest(ApiWithPostgresAssemblyFixture fixture)
{
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task StoreRobotConfiguration_ReturnsEmptyDraft()
    {
        using var client = fixture.CreateClient();

        var configuration = await client.StoreRobotConfigurationAsync();

        configuration.Status.ShouldBe(RobotConfigurationStatusEnum.Draft);
        configuration.Parts.ShouldBeEmpty();
    }

    [Fact]
    public async Task AddPart_SeededParts_AppearInConfigurationWithTheirSnapshot()
    {
        using var client = fixture.CreateClient();

        var configurationId = await client.BuildRobotAsync("Obudowa Ranger (PETG)", "Napęd Trail 12V");

        var configuration = await client.GetFromJsonAsync<RobotConfigurationResource>(
            $"/api/configurations/{configurationId}", ApiJsonSerializerOptions.Web, CancellationToken);
        configuration!.Parts.Select(part => part.Name).ShouldBe(["Obudowa Ranger (PETG)", "Napęd Trail 12V"]);
        configuration.Parts[1].PriceAmount.ShouldBe(189m);
    }

    [Fact]
    public async Task ShowQuote_CompatibleRobot_IsValidWithPriceAndLeadTime()
    {
        using var client = fixture.CreateClient();
        var configurationId = await client.BuildRobotAsync("Obudowa Ranger (PETG)", "Napęd Trail 12V", "Sterownik Pro 12V", "Kamera 5 MP");

        var quote = await client.ShowQuoteAsync(configurationId);

        quote.IsValid.ShouldBeTrue();
        quote.PartsPriceAmount.ShouldBe(686m);
        quote.AssemblyPriceAmount.ShouldBe(102.90m);
        quote.DiscountAmount.ShouldBe(0m);
        quote.TotalPriceAmount.ShouldBe(788.90m);
        quote.LeadTimeWorkingDays.ShouldBe(8);
    }

    [Fact]
    public async Task ShowQuote_DriveThatDoesNotFitHousing_ReportsR2AndR4()
    {
        using var client = fixture.CreateClient();
        var configurationId = await client.BuildRobotAsync("Obudowa Titan (ASA)", "Napęd Micro 6V", "Sterownik Nano 6V");

        var quote = await client.ShowQuoteAsync(configurationId);

        quote.IsValid.ShouldBeFalse();
        quote.Violations.Select(violation => violation.Rule).ShouldBe(["R2", "R4"]);
    }

    [Fact]
    public async Task ShowQuote_RobotFromThousandZloty_GetsDiscount()
    {
        using var client = fixture.CreateClient();
        var configurationId = await client.BuildRobotAsync("Obudowa Titan (ASA)", "Napęd Crawler 12V", "Sterownik Edge AI 12V", "Lidar 360°");

        var quote = await client.ShowQuoteAsync(configurationId);

        quote.IsValid.ShouldBeTrue();
        quote.DiscountAmount.ShouldBe(95.22m);
        quote.TotalPriceAmount.ShouldBe(1809.18m);
        quote.LeadTimeWorkingDays.ShouldBe(13);
    }

    [Fact]
    public async Task ShowQuote_EmptyConfiguration_ReportsMissingPartsAsR1()
    {
        using var client = fixture.CreateClient();
        var configuration = await client.StoreRobotConfigurationAsync();

        var quote = await client.ShowQuoteAsync(configuration.Id);

        quote.Violations.Select(violation => violation.Rule).ShouldBe(["R1", "R1", "R1"]);
    }

    [Fact]
    public async Task RemovePart_PartInConfiguration_RemovesIt()
    {
        using var client = fixture.CreateClient();
        var cameraId = await client.FindSeededPartIdAsync("Kamera 5 MP");
        var configurationId = await client.BuildRobotAsync("Kamera 5 MP");

        var response = await client.DeleteAsync($"/api/configurations/{configurationId}/parts/{cameraId}", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var configuration = await response.Content.ReadFromJsonAsync<RobotConfigurationResource>(ApiJsonSerializerOptions.Web, CancellationToken);
        configuration!.Parts.ShouldBeEmpty();
    }

    [Fact]
    public async Task RemovePart_PartNotInConfiguration_ReturnsUnprocessableEntityWithRuleS2()
    {
        using var client = fixture.CreateClient();
        var configuration = await client.StoreRobotConfigurationAsync();

        var response = await client.DeleteAsync($"/api/configurations/{configuration.Id}/parts/{Guid.NewGuid()}", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["S2"]);
    }

    [Fact]
    public async Task AddPart_UnknownPart_ReturnsNotFound()
    {
        using var client = fixture.CreateClient();
        var configuration = await client.StoreRobotConfigurationAsync();

        var response = await client.AddPartAsync(configuration.Id, Guid.NewGuid());

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShowQuote_UnknownConfiguration_ReturnsNotFound()
    {
        using var client = fixture.CreateClient();

        var response = await client.GetAsync($"/api/configurations/{Guid.NewGuid()}/quote", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
