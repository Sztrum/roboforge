using Ardalis.GuardClauses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Infrastructure.Database;
using RoboForge.Shared.Hosting;

namespace RoboForge.Modules.Sales.Infrastructure;

public static class SalesModuleServiceRegistration
{
    /// <summary>Registers everything the Sales module needs: its database context and request validators.</summary>
    public static void AddSalesModule(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(AspireResourceNames.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{AspireResourceNames.Database}' not found.");

        builder.Services.AddDbContext<SalesDbContext>(options => options
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", SalesDbContext.Schema)));

        builder.EnrichNpgsqlDbContext<SalesDbContext>();

        builder.Services.AddScoped<ISalesDbContext>(services => services.GetRequiredService<SalesDbContext>());

        builder.Services.AddValidatorsFromAssembly(typeof(ISalesDbContext).Assembly, includeInternalTypes: true);
    }
}
