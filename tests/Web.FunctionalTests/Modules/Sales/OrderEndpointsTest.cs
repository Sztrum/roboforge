using System.Net;
using System.Net.Http.Json;
using RoboForge.Modules.Sales.Application.Orders.Resources;
using RoboForge.Modules.Sales.Domain.Orders.Enums;
using static RoboForge.Web.FunctionalTests.Infrastructure.RoboForgeApiTestSteps;

namespace RoboForge.Web.FunctionalTests.Modules.Sales;

public class OrderEndpointsTest(ApiWithPostgresAssemblyFixture fixture)
{
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task PlaceOrder_ValidConfiguration_ReturnsPlacedOrderWithFrozenPrice()
    {
        using var client = fixture.CreateClient();

        var order = await client.PlaceRangerRobotOrderAsync();

        order.Status.ShouldBe(OrderStatusEnum.Placed);
        order.Parts.Count.ShouldBe(4);
        order.TotalPriceAmount.ShouldBe(788.90m);
        order.LeadTimeWorkingDays.ShouldBe(8);
        var shownOrder = await client.ShowOrderAsync(order.Id);
        shownOrder.PlacedOnUtc.ShouldBe(order.PlacedOnUtc, TimeSpan.FromMilliseconds(1)); // PostgreSQL keeps microseconds
        (shownOrder with { PlacedOnUtc = order.PlacedOnUtc }).ShouldBeEquivalentTo(order);
    }

    [Fact]
    public async Task PlaceOrder_DriveThatDoesNotFitHousing_ReturnsUnprocessableEntityWithR2AndR4()
    {
        using var client = fixture.CreateClient();
        var configurationId = await client.BuildRobotAsync("Obudowa Titan (ASA)", "Napęd Micro 6V", "Sterownik Nano 6V");

        var response = await client.PlaceOrderAsync(configurationId);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["R2", "R4"]);
    }

    [Fact]
    public async Task PlaceOrder_SameConfigurationTwice_ReturnsUnprocessableEntityWithRuleS1()
    {
        using var client = fixture.CreateClient();
        var order = await client.PlaceRangerRobotOrderAsync();

        var response = await client.PlaceOrderAsync(order.ConfigurationId);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["S1"]);
    }

    [Fact]
    public async Task ConfirmOrder_PlacedOrder_ConfirmsIt()
    {
        using var client = fixture.CreateClient();
        var order = await client.PlaceRangerRobotOrderAsync();

        var confirmedOrder = await client.ConfirmOrderAsync(order.Id);

        confirmedOrder.Status.ShouldBe(OrderStatusEnum.Confirmed);
    }

    [Fact]
    public async Task CancelOrder_PlacedOrder_CancelsItWithReason()
    {
        using var client = fixture.CreateClient();
        var order = await client.PlaceRangerRobotOrderAsync();

        var response = await client.CancelOrderAsync(order.Id, "Zmiana planów");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var cancelledOrder = await response.Content.ReadFromJsonAsync<OrderResource>(ApiJsonSerializerOptions.Web, CancellationToken);
        cancelledOrder!.Status.ShouldBe(OrderStatusEnum.Cancelled);
        cancelledOrder.CancellationReason.ShouldBe("Zmiana planów");
    }

    [Fact]
    public async Task CancelOrder_ConfirmedOrder_ReturnsUnprocessableEntityWithRuleR8()
    {
        using var client = fixture.CreateClient();
        var order = await client.PlaceRangerRobotOrderAsync();
        await client.ConfirmOrderAsync(order.Id);

        var response = await client.CancelOrderAsync(order.Id, "Za późno");

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["R8"]);
    }

    [Fact]
    public async Task CancelOrder_WithoutReason_ReturnsBadRequest()
    {
        using var client = fixture.CreateClient();
        var order = await client.PlaceRangerRobotOrderAsync();

        var response = await client.CancelOrderAsync(order.Id, "");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task IndexOrders_AfterPlacingOrder_ListsIt()
    {
        using var client = fixture.CreateClient();
        var order = await client.PlaceRangerRobotOrderAsync();

        var orders = await client.GetFromJsonAsync<CollectionOrderResource>("/api/orders", ApiJsonSerializerOptions.Web, CancellationToken);

        orders!.Items.Select(listedOrder => listedOrder.Id).ShouldContain(order.Id);
    }

    [Fact]
    public async Task ShowOrder_UnknownOrder_ReturnsNotFound()
    {
        using var client = fixture.CreateClient();

        var response = await client.GetAsync($"/api/orders/{Guid.NewGuid()}", CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
