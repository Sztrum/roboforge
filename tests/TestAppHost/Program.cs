using RoboForge.Shared.Hosting;

namespace RoboForge.TestAppHost;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = DistributedApplication.CreateBuilder(args);

        builder.AddPostgres(AspireResourceNames.DatabaseServer)
            .AddDatabase(AspireResourceNames.Database);

        await builder.Build().RunAsync();
    }
}
