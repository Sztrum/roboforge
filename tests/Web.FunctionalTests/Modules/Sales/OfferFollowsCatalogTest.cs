using System.Net;
using System.Net.Http.Json;
using RoboForge.Modules.Sales.Application.Configurations.Resources;

namespace RoboForge.Web.FunctionalTests.Modules.Sales;

/// <summary>Sales learns about catalog changes only from Catalog's integration events (through the outbox).</summary>
public class OfferFollowsCatalogTest(ApiWithPostgresAssemblyFixture fixture)
{
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task AddPart_AfterCatalogDiscontinuedIt_ReturnsUnprocessableEntityWithRuleR7()
    {
        using var client = fixture.CreateClient();
        var sensor = await client.StoreSensorAsync();
        await client.PostAsync($"/api/parts/{sensor.Id}/discontinue", null, CancellationToken);
        await fixture.WaitUntilSalesOffersPartAsync(sensor.Id, offeredPart => offeredPart.CurrentOffer.IsDiscontinued);
        var configuration = await client.StoreRobotConfigurationAsync();

        var response = await client.AddPartAsync(configuration.Id, sensor.Id);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["R7"]);
    }

    [Fact]
    public async Task ShowQuote_DraftWithPartCatalogDiscontinuedLater_ReportsR7()
    {
        using var client = fixture.CreateClient();
        var sensor = await client.StoreSensorAsync();
        await fixture.WaitUntilSalesOffersPartAsync(sensor.Id);
        var configurationId = await client.BuildRobotAsync(RoboForgeApiTestSteps.RangerHousing, RoboForgeApiTestSteps.TrailDrive, RoboForgeApiTestSteps.ProElectronics);
        await client.AddPartAsync(configurationId, sensor.Id);

        await client.PostAsync($"/api/parts/{sensor.Id}/discontinue", null, CancellationToken);

        var quote = await EventuallyConsistentAssertion.WaitUntilAsync(
            () => client.ShowQuoteAsync(configurationId),
            currentQuote => !currentQuote.IsValid,
            "the draft to report the discontinued sensor");
        quote.Violations.Select(violation => violation.Rule).ShouldBe(["R7"]);
    }

    [Fact]
    public async Task ChangePartPrice_InCatalog_AppliesToNewSnapshotsButNotToExistingOnes()
    {
        using var client = fixture.CreateClient();
        var sensor = await client.StoreSensorAsync(priceAmount: 50m);
        await fixture.WaitUntilSalesOffersPartAsync(sensor.Id);
        var configurationBeforePriceChange = await client.StoreRobotConfigurationAsync();
        await client.AddPartAsync(configurationBeforePriceChange.Id, sensor.Id);

        await client.PutAsJsonAsync($"/api/parts/{sensor.Id}/price", new { priceAmount = 70m }, CancellationToken);
        await fixture.WaitUntilSalesOffersPartAsync(sensor.Id, offeredPart => offeredPart.CurrentOffer.Price.Amount == 70m);
        var configurationAfterPriceChange = await client.StoreRobotConfigurationAsync();
        await client.AddPartAsync(configurationAfterPriceChange.Id, sensor.Id);

        (await PriceOfOnlyPartAsync(client, configurationBeforePriceChange.Id)).ShouldBe(50m);
        (await PriceOfOnlyPartAsync(client, configurationAfterPriceChange.Id)).ShouldBe(70m);
    }

    private static async Task<decimal> PriceOfOnlyPartAsync(HttpClient client, Guid configurationId)
    {
        var configuration = await client.GetFromJsonAsync<RobotConfigurationResource>(
            $"/api/configurations/{configurationId}", ApiJsonSerializerOptions.Web, CancellationToken);

        return configuration!.Parts.Single().PriceAmount;
    }
}
