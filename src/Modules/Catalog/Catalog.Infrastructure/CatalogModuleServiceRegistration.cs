using Ardalis.GuardClauses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoboForge.Modules.Catalog.Application.Database;
using RoboForge.Modules.Catalog.Infrastructure.Database;
using RoboForge.Modules.Catalog.Infrastructure.Database.Seeding;
using RoboForge.Shared.Hosting;

namespace RoboForge.Modules.Catalog.Infrastructure;

public static class CatalogModuleServiceRegistration
{
    /// <summary>Registers everything the Catalog module needs: its database context and request validators.</summary>
    public static void AddCatalogModule(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(AspireResourceNames.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{AspireResourceNames.Database}' not found.");

        builder.Services.AddDbContext<CatalogDbContext>(options => options
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", CatalogDbContext.Schema))
            .UseAsyncSeeding((dbContext, _, cancellationToken) =>
                CatalogStarterPartsSeeder.SeedWhenEmptyAsync(dbContext, cancellationToken)));

        builder.EnrichNpgsqlDbContext<CatalogDbContext>();

        builder.Services.AddScoped<ICatalogDbContext>(services => services.GetRequiredService<CatalogDbContext>());

        builder.Services.AddValidatorsFromAssembly(typeof(ICatalogDbContext).Assembly, includeInternalTypes: true);
    }
}
