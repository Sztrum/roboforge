using System.Net;

namespace RoboForge.Application.FunctionalTests.Web;

public class HealthEndpointTest(ApiWithPostgresAssemblyFixture fixture)
{
    [Fact]
    public async Task Health_WhenDatabaseIsUp_ReturnsHealthy()
    {
        using var client = fixture.CreateClient();

        var response = await client.GetAsync("/health", TestContext.Current.CancellationToken);

        response.StatusCode.ShouldBe(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken)).ShouldBe("Healthy");
    }
}
