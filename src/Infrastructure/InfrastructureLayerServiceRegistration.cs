using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using RoboForge.Application.Common.Interfaces;
using RoboForge.Infrastructure.Data;

namespace Microsoft.Extensions.DependencyInjection;

public static class InfrastructureLayerServiceRegistration
{
    public static void AddInfrastructureServices(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(AspireResourceNames.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{AspireResourceNames.Database}' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));

        builder.EnrichNpgsqlDbContext<ApplicationDbContext>();

        builder.Services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        builder.Services.AddScoped<ApplyPendingMigrationsDatabaseInitialiser>();
    }
}
