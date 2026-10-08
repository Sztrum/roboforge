[assembly: AssemblyFixture(typeof(FunctionalTestFixture))]

namespace RoboForge.Application.FunctionalTests.Infrastructure;

/// <summary>
/// Starts PostgreSQL through the test Aspire app host and the Web API against it,
/// once for the whole test assembly.
/// </summary>
public sealed class FunctionalTestFixture : IAsyncLifetime
{
    private DistributedApplication? _app;
    private WebApiFactory? _factory;

    public HttpClient CreateClient() => _factory!.CreateClient();

    public async ValueTask InitializeAsync()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMinutes(2));
        var cancellationToken = cts.Token;

        var builder = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.TestAppHost>(
                args: [],
                configureBuilder: (options, _) => options.DisableDashboard = true,
                cancellationToken);

        builder.Configuration["ASPIRE_ALLOW_UNSECURED_TRANSPORT"] = "true";

        _app = await builder.BuildAsync(cancellationToken);
        await _app.StartAsync(cancellationToken);
        await _app.ResourceNotifications.WaitForResourceHealthyAsync(Services.Database, cancellationToken);

        var connectionString = (await _app.GetConnectionStringAsync(Services.Database, cancellationToken))!;

        _factory = new WebApiFactory(connectionString);
    }

    public async ValueTask DisposeAsync()
    {
        if (_factory is not null) await _factory.DisposeAsync();
        if (_app is not null) await _app.DisposeAsync();
    }
}
