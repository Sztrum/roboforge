using RoboForge.AppHost;
using RoboForge.Shared;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder
    .AddPostgres(AspireResourceNames.DatabaseServer)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase(AspireResourceNames.Database);

builder.AddProject<Projects.Web>(AspireResourceNames.WebApi)
    .WithReference(database)
    .WaitFor(database)
    .WithExternalHttpEndpoints()
    .WithAspNetCoreEnvironment()
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

await builder.Build().RunAsync();
