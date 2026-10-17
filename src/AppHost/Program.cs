using RoboForge.AppHost;
using RoboForge.Shared.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var database = builder
    .AddPostgres(AspireResourceNames.DatabaseServer)
    .WithLifetime(ContainerLifetime.Persistent)
    .AddDatabase(AspireResourceNames.Database);

var webApi = builder.AddProject<Projects.Web>(AspireResourceNames.WebApi)
    .WithReference(database)
    .WaitFor(database)
    .WithExternalHttpEndpoints()
    .WithAspNetCoreEnvironment()
    .WithUrlForEndpoint("http", url =>
    {
        url.DisplayText = "Scalar API Reference";
        url.Url = "/scalar";
    });

builder.AddProject<Projects.Frontend>(AspireResourceNames.Frontend)
    .WithReference(webApi)
    .WaitFor(webApi)
    .WithExternalHttpEndpoints()
    .WithAspNetCoreEnvironment();

await builder.Build().RunAsync();
