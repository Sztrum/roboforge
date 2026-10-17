using System.Net;
using System.Net.Http.Json;
using RoboForge.Modules.Catalog.Application.Parts.Resources;
using RoboForge.Modules.Catalog.Domain.Parts.Enums;

namespace RoboForge.Web.FunctionalTests.Modules.Catalog;

public class PartIndexEndpointTest(ApiWithPostgresAssemblyFixture fixture)
{
    [Fact]
    public async Task IndexParts_WithoutCategory_ReturnsSeededPartsOfEveryCategory()
    {
        using var client = fixture.CreateClient();

        var response = await client.GetAsync("/api/parts", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        var parts = await response.Content.ReadFromJsonAsync<CollectionPartResource>(
            ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);
        parts!.Items.Select(part => part.Name).ShouldContain("Napęd Trail 12V");
        parts.Items.Select(part => part.Category).Distinct().Count().ShouldBe(4);
    }

    [Fact]
    public async Task IndexParts_WithCategory_ReturnsOnlyThatCategory()
    {
        using var client = fixture.CreateClient();

        var drives = await client.GetFromJsonAsync<CollectionPartResource>(
            "/api/parts?category=Drive", ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);

        drives!.Items.ShouldNotBeEmpty();
        drives.Items.ShouldAllBe(part => part.Category == PartCategoryEnum.Drive);
    }

    [Fact]
    public async Task IndexParts_SeededDrive_HasPriceAndTechSpec()
    {
        using var client = fixture.CreateClient();

        var drives = await client.GetFromJsonAsync<CollectionPartResource>(
            "/api/parts?category=Drive", ApiJsonSerializerOptions.Web, TestContext.Current.CancellationToken);

        var trailDrive = drives!.Items.First(part => part.Name == "Napęd Trail 12V");
        trailDrive.PriceAmount.ShouldBe(189m);
        trailDrive.PriceCurrency.ShouldBe("PLN");
        trailDrive.MountType.ShouldBe(MountTypeEnum.Standard);
        trailDrive.VoltageVolts.ShouldBe(12);
        trailDrive.MaxLoadGrams.ShouldBe(2000);
    }
}
