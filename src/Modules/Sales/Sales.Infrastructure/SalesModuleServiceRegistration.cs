using Ardalis.GuardClauses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoboForge.Modules.Sales.Application.Database;
using RoboForge.Modules.Sales.Application.IntegrationEvents;
using RoboForge.Modules.Sales.Infrastructure.Database;
using RoboForge.Shared.Hosting;
using RoboForge.Shared.Infrastructure.Outbox;

namespace RoboForge.Modules.Sales.Infrastructure;

public static class SalesModuleServiceRegistration
{
    /// <summary>
    /// Registers everything the Sales module needs: its database context (with the outbox), the outbox
    /// publisher and request validators.
    /// </summary>
    public static void AddSalesModule(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(AspireResourceNames.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{AspireResourceNames.Database}' not found.");

        builder.Services.AddDbContext<SalesDbContext>(options => options
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", SalesDbContext.Schema))
            .AddInterceptors(new WriteIntegrationEventsToOutboxSaveChangesInterceptor(SalesDomainEventToIntegrationEventMapper.Map)));

        builder.EnrichNpgsqlDbContext<SalesDbContext>();

        builder.Services.AddScoped<ISalesDbContext>(services => services.GetRequiredService<SalesDbContext>());

        builder.Services.AddHostedService<PublishOutboxMessagesHostedService<SalesDbContext>>();

        builder.Services.AddValidatorsFromAssembly(typeof(ISalesDbContext).Assembly, includeInternalTypes: true);
    }
}
