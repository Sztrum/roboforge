using RoboForge.Shared;

namespace RoboForge.TestAppHost;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddPostgres(Services.DatabaseServer)
            .AddDatabase(Services.Database);

        await builder.Build().RunAsync();
    }
}
