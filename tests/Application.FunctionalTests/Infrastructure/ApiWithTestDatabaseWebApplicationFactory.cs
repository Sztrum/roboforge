using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace RoboForge.Application.FunctionalTests.Infrastructure;

public sealed class ApiWithTestDatabaseWebApplicationFactory(string connectionString) : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting($"ConnectionStrings:{AspireResourceNames.Database}", connectionString);
    }
}
