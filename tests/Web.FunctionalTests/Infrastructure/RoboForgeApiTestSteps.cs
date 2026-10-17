using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RoboForge.Modules.Catalog.Application.Parts.Commands;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;
using RoboForge.Modules.Sales.Application.Configurations.Resources;
using RoboForge.Modules.Sales.Application.Orders.Resources;
using RoboForge.Modules.Sales.Domain.Parts;
using RoboForge.Modules.Sales.Infrastructure.Database;

namespace RoboForge.Web.FunctionalTests.Infrastructure;

/// <summary>The API calls the scenario tests are made of, so each test reads like the demo.</summary>
internal static class RoboForgeApiTestSteps
{
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    public static async Task<Guid> FindSeededPartIdAsync(this HttpClient client, string partName)
    {
        var parts = await client.GetFromJsonAsync<CollectionPartResource>("/api/parts", ApiJsonSerializerOptions.Web, CancellationToken);

        return parts!.Items.Single(part => part.Name == partName).Id;
    }

    public static async Task<PartResource> StoreSensorAsync(this HttpClient client, decimal priceAmount = 49.99m)
    {
        var sensor = new StorePartCommand($"Czujnik testowy {Guid.NewGuid():N}", PartCategoryEnum.Sensor, priceAmount, 2, 20, null, null, null);
        var response = await client.PostAsJsonAsync("/api/parts", sensor, ApiJsonSerializerOptions.Web, CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        return (await response.Content.ReadFromJsonAsync<PartResource>(ApiJsonSerializerOptions.Web, CancellationToken))!;
    }

    public static async Task<RobotConfigurationResource> StoreRobotConfigurationAsync(this HttpClient client)
    {
        var response = await client.PostAsync("/api/configurations", null, CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        return (await response.Content.ReadFromJsonAsync<RobotConfigurationResource>(ApiJsonSerializerOptions.Web, CancellationToken))!;
    }

    public static Task<HttpResponseMessage> AddPartAsync(this HttpClient client, Guid configurationId, Guid partId) =>
        client.PostAsJsonAsync($"/api/configurations/{configurationId}/parts", new { partId }, CancellationToken);

    /// <summary>Starts a configuration and adds the seeded parts with these names.</summary>
    public static async Task<Guid> BuildRobotAsync(this HttpClient client, params string[] partNames)
    {
        var configuration = await client.StoreRobotConfigurationAsync();

        foreach (var partName in partNames)
        {
            var response = await client.AddPartAsync(configuration.Id, await client.FindSeededPartIdAsync(partName));
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        return configuration.Id;
    }

    public static async Task<RobotQuoteResource> ShowQuoteAsync(this HttpClient client, Guid configurationId) =>
        (await client.GetFromJsonAsync<RobotQuoteResource>(
            $"/api/configurations/{configurationId}/quote", ApiJsonSerializerOptions.Web, CancellationToken))!;

    public const string RangerHousing = "Obudowa Ranger (PETG)";
    public const string TrailDrive = "Napęd Trail 12V";
    public const string ProElectronics = "Sterownik Pro 12V";
    public const string Camera = "Kamera 5 MP";

    public static Task<HttpResponseMessage> PlaceOrderAsync(this HttpClient client, Guid configurationId) =>
        client.PostAsJsonAsync("/api/orders", new { configurationId }, CancellationToken);

    /// <summary>Builds the compatible Ranger robot (788.90 PLN, 8 working days) and places an order for it.</summary>
    public static async Task<OrderResource> PlaceRangerRobotOrderAsync(this HttpClient client)
    {
        var configurationId = await client.BuildRobotAsync(RangerHousing, TrailDrive, ProElectronics, Camera);
        var response = await client.PlaceOrderAsync(configurationId);
        response.StatusCode.ShouldBe(HttpStatusCode.Created);

        return (await response.Content.ReadFromJsonAsync<OrderResource>(ApiJsonSerializerOptions.Web, CancellationToken))!;
    }

    public static async Task<OrderResource> ConfirmOrderAsync(this HttpClient client, Guid orderId)
    {
        var response = await client.PostAsync($"/api/orders/{orderId}/confirm", null, CancellationToken);
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        return (await response.Content.ReadFromJsonAsync<OrderResource>(ApiJsonSerializerOptions.Web, CancellationToken))!;
    }

    public static Task<HttpResponseMessage> CancelOrderAsync(this HttpClient client, Guid orderId, string reason) =>
        client.PostAsJsonAsync($"/api/orders/{orderId}/cancel", new { reason }, CancellationToken);

    public static async Task<OrderResource> ShowOrderAsync(this HttpClient client, Guid orderId) =>
        (await client.GetFromJsonAsync<OrderResource>($"/api/orders/{orderId}", ApiJsonSerializerOptions.Web, CancellationToken))!;

    /// <summary>Waits until Sales' copy of the offer has caught up with the catalog (it arrives through the outbox).</summary>
    public static Task WaitUntilSalesOffersPartAsync(
        this ApiWithPostgresAssemblyFixture fixture,
        Guid partId,
        Func<OfferedPartAggregate, bool>? isUpToDate = null) =>
        EventuallyConsistentAssertion.WaitUntilAsync(
            async () =>
            {
                await using var scope = fixture.CreateServiceScope();
                var salesDbContext = scope.ServiceProvider.GetRequiredService<SalesDbContext>();

                return await salesDbContext.OfferedParts.AsNoTracking()
                    .FirstOrDefaultAsync(part => part.Id == new CatalogPartId(partId), CancellationToken);
            },
            offeredPart => offeredPart is not null && (isUpToDate?.Invoke(offeredPart) ?? true),
            $"Sales to offer part {partId}");
}
