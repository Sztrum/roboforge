using System.Net;
using RoboForge.Modules.Sales.Domain.Orders.Enums;
using ProductionStageEnum = RoboForge.Modules.Production.Domain.Jobs.Enums.ProductionStageEnum;
using SalesProductionStageEnum = RoboForge.Modules.Sales.Domain.Orders.Enums.ProductionStageEnum;

namespace RoboForge.Web.FunctionalTests;

/// <summary>
/// The demo from docs/PROJECT.md, end to end: the order follows the workshop by itself, only through
/// events between Sales and Production.
/// </summary>
public class DemoScenarioTest(ApiWithPostgresAssemblyFixture fixture)
{
    [Fact]
    public async Task ConfirmedOrder_GoesThroughProductionWithFailedQa_AndEndsShippedWithLongerLeadTime()
    {
        using var client = fixture.CreateClient();
        var order = await client.PlaceRangerRobotOrderAsync();
        await client.ConfirmOrderAsync(order.Id);
        var job = await client.WaitForProductionJobOfOrderAsync(order.Id);

        await client.CompleteStagesAsync(job.Id, ProductionStageEnum.Printing, ProductionStageEnum.Assembly);
        (await client.FailQaAsync(job.Id, "Luźne mocowanie napędu")).StatusCode.ShouldBe(HttpStatusCode.OK);
        var orderAfterFailedQa = await client.WaitForOrderAsync(
            order.Id, current => current.Notes.Count == 1, "the failed QA to reach the order");

        orderAfterFailedQa.Status.ShouldBe(OrderStatusEnum.InProduction);
        orderAfterFailedQa.LeadTimeWorkingDays.ShouldBe(8 + 3);
        (await client.CancelOrderAsync(order.Id, "Jednak nie")).StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);

        await client.CompleteStagesAsync(job.Id, ProductionStageEnum.Assembly, ProductionStageEnum.QaTests, ProductionStageEnum.Packing);
        (await client.ShipAsync(job.Id, "RF-2026-0001")).StatusCode.ShouldBe(HttpStatusCode.OK);
        var shippedOrder = await client.WaitForOrderAsync(
            order.Id, current => current.Status == OrderStatusEnum.Shipped, "the order to be shipped");

        shippedOrder.TrackingNumber.ShouldBe("RF-2026-0001");
        shippedOrder.CompletedStages.Select(entry => entry.Stage).ShouldBe(
        [
            SalesProductionStageEnum.Printing,
            SalesProductionStageEnum.Assembly,
            SalesProductionStageEnum.Assembly,
            SalesProductionStageEnum.QaTests,
            SalesProductionStageEnum.Packing,
            SalesProductionStageEnum.Shipped,
        ]);
    }
}
