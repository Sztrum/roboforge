using System.Net;
using System.Net.Http.Json;
using RoboForge.Modules.Production.Application.Jobs.Resources;
using RoboForge.Modules.Production.Domain.Jobs.Enums;

namespace RoboForge.Web.FunctionalTests.Modules.Production;

public class ProductionJobEndpointsTest(ApiWithPostgresAssemblyFixture fixture)
{
    private static CancellationToken CancellationToken => TestContext.Current.CancellationToken;

    [Fact]
    public async Task ConfirmOrder_StartsProductionJobAtPrintingWithOrderParts()
    {
        using var client = fixture.CreateClient();

        var job = await client.StartRangerRobotProductionAsync();

        job.CurrentStage.ShouldBe(ProductionStageEnum.Printing);
        job.Status.ShouldBe(ProductionJobStatusEnum.InProgress);
        job.Parts.Count.ShouldBe(4);
    }

    [Fact]
    public async Task CompleteStage_SkippingAStage_ReturnsUnprocessableEntityWithRuleR9()
    {
        using var client = fixture.CreateClient();
        var job = await client.StartRangerRobotProductionAsync();

        var response = await client.CompleteStageAsync(job.Id, ProductionStageEnum.Assembly);

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["R9"]);
    }

    [Fact]
    public async Task FailQa_Twice_PutsJobOnHoldUntilReleased()
    {
        using var client = fixture.CreateClient();
        var job = await client.StartRangerRobotProductionAsync();
        await client.CompleteStagesAsync(job.Id, ProductionStageEnum.Printing, ProductionStageEnum.Assembly);
        await client.FailQaAsync(job.Id, "Luźne mocowanie napędu");
        await client.CompleteStagesAsync(job.Id, ProductionStageEnum.Assembly);
        await client.FailQaAsync(job.Id, "Kamera nie odpowiada");

        var blockedResponse = await client.CompleteStageAsync(job.Id, ProductionStageEnum.Assembly);
        var releaseResponse = await client.PostAsync($"/api/production/jobs/{job.Id}/release-hold", null, CancellationToken);
        var resumedResponse = await client.CompleteStageAsync(job.Id, ProductionStageEnum.Assembly);

        blockedResponse.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await blockedResponse.ReadViolatedRulesAsync()).ShouldBe(["R10"]);
        releaseResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
        var resumedJob = await resumedResponse.Content.ReadFromJsonAsync<ProductionJobResource>(ApiJsonSerializerOptions.Web, CancellationToken);
        resumedJob!.CurrentStage.ShouldBe(ProductionStageEnum.QaTests);
        resumedJob.QaFailures.Count.ShouldBe(2);
    }

    [Fact]
    public async Task IndexProductionJobs_WithStage_ReturnsOnlyJobsAtThatStage()
    {
        using var client = fixture.CreateClient();
        var job = await client.StartRangerRobotProductionAsync();
        await client.CompleteStagesAsync(job.Id, ProductionStageEnum.Printing);

        var assemblyQueue = await client.GetFromJsonAsync<CollectionProductionJobResource>(
            "/api/production/jobs?stage=Assembly", ApiJsonSerializerOptions.Web, CancellationToken);

        assemblyQueue!.Items.Select(queuedJob => queuedJob.Id).ShouldContain(job.Id);
        assemblyQueue.Items.ShouldAllBe(queuedJob => queuedJob.CurrentStage == ProductionStageEnum.Assembly);
    }

    [Fact]
    public async Task Ship_BeforePacking_ReturnsUnprocessableEntityWithRuleR9()
    {
        using var client = fixture.CreateClient();
        var job = await client.StartRangerRobotProductionAsync();

        var response = await client.ShipAsync(job.Id, "RF-000001");

        response.StatusCode.ShouldBe(HttpStatusCode.UnprocessableEntity);
        (await response.ReadViolatedRulesAsync()).ShouldBe(["R9"]);
    }
}
