using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoboForge.Modules.Catalog.Infrastructure.Database;
using RoboForge.Shared.Hosting;

namespace RoboForge.Modules.Catalog.Infrastructure;

public static class CatalogModuleServiceRegistration
{
    /// <summary>Registers everything the Catalog module needs: its database context for now.</summary>
    public static void AddCatalogModule(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(AspireResourceNames.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{AspireResourceNames.Database}' not found.");

        builder.Services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CatalogDbContext.Schema)));

        builder.EnrichNpgsqlDbContext<CatalogDbContext>();
    }
}
