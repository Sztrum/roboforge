using System.Net;
using System.Net.Http.Json;
using RoboForge.Modules.Catalog.Application.Parts.Commands;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;

namespace RoboForge.Web.FunctionalTests.Modules.Catalog;

public class PartCommandEndpointsTest(ApiWithPostgresAssemblyFixture fixture)
{
    private static StorePartCommand NewSensor(string name) =>
        new(name, PartCategoryEnum.Sensor, 49.99m, 2, 20, MountType: null, VoltageVolts: null, MaxLoadGrams: null);

    private static async Task<PartResource> StoreSensorAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync(
            "/api/parts", NewSensor($"Czujnik testowy {Guid.NewGuid():N}"), ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        return (await response.Content.ReadFromJsonAsync<PartResource>(ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken))!;
    }

    [Fact]
    public async Task StorePart_WithValidPart_ReturnsCreatedPartThatCanBeShown()
    {
        using var client = fixture.CreateClient();

        var storedSensor = await StoreSensorAsync(client);

        var shownSensor = await client.GetFromJsonAsync<PartResource>(
            $"/api/parts/{storedSensor.Id}", ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        shownSensor.ShouldBe(storedSensor);
        shownSensor!.PriceAmount.ShouldBe(49.99m);
        shownSensor.IsDiscontinued.ShouldBeFalse();
    }

    [Fact]
    public async Task StorePart_WithEmptyName_ReturnsBadRequest()
    {
        using var client = fixture.CreateClient();

        var response = await client.PostAsJsonAsync("/api/parts", NewSensor(""), ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task StorePart_DriveWithoutMountType_ReturnsUnprocessableEntityWithRuleC3()
    {
        using var client = fixture.CreateClient();
        var driveWithoutMountType = new StorePartCommand(
            "Napęd bez mocowania", PartCategoryEnum.Drive, 100m, 3, 300, MountType: null, VoltageVolts: 12, MaxLoadGrams: 1000);

        var response = await client.PostAsJsonAsync("/api/parts", driveWithoutMountType, ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["C3"]);
    }

    [Fact]
    public async Task StorePart_WithNegativePrice_ReturnsUnprocessableEntityWithRuleM1()
    {
        using var client = fixture.CreateClient();
        var sensorWithNegativePrice = NewSensor("Czujnik z ujemną ceną") with { PriceAmount = -1m };

        var response = await client.PostAsJsonAsync("/api/parts", sensorWithNegativePrice, ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["M1"]);
    }

    [Fact]
    public async Task ChangePartPrice_OfOfferedPart_ReturnsPartWithNewPrice()
    {
        using var client = fixture.CreateClient();
        var sensor = await StoreSensorAsync(client);

        var response = await client.PutAsJsonAsync(
            $"/api/parts/{sensor.Id}/price", new { priceAmount = 39.995m }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var repricedSensor = await response.Content.ReadFromJsonAsync<PartResource>(ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        repricedSensor!.PriceAmount.ShouldBe(40.00m);
    }

    [Fact]
    public async Task ChangePartPrice_OfUnknownPart_ReturnsNotFound()
    {
        using var client = fixture.CreateClient();

        var response = await client.PutAsJsonAsync(
            $"/api/parts/{Guid.NewGuid()}/price", new { priceAmount = 10m }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DiscontinuePart_OfferedPart_MarksItDiscontinued()
    {
        using var client = fixture.CreateClient();
        var sensor = await StoreSensorAsync(client);

        var response = await client.PostAsync($"/api/parts/{sensor.Id}/discontinue", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var discontinuedSensor = await response.Content.ReadFromJsonAsync<PartResource>(ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        discontinuedSensor!.IsDiscontinued.ShouldBeTrue();
    }

    [Fact]
    public async Task DiscontinuePart_Twice_ReturnsUnprocessableEntityWithRuleC5()
    {
        using var client = fixture.CreateClient();
        var sensor = await StoreSensorAsync(client);
        await client.PostAsync($"/api/parts/{sensor.Id}/discontinue", null, TestContext.Current.CancellationToken);

        var response = await client.PostAsync($"/api/parts/{sensor.Id}/discontinue", null, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["C5"]);
    }

    [Fact]
    public async Task ChangePartPrice_OfDiscontinuedPart_ReturnsUnprocessableEntityWithRuleC4()
    {
        using var client = fixture.CreateClient();
        var sensor = await StoreSensorAsync(client);
        await client.PostAsync($"/api/parts/{sensor.Id}/discontinue", null, TestContext.Current.CancellationToken);

        var response = await client.PutAsJsonAsync(
            $"/api/parts/{sensor.Id}/price", new { priceAmount = 10m }, TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["C4"]);
    }
}
