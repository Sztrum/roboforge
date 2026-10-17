using Ardalis.GuardClauses;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RoboForge.Modules.Production.Application.Database;
using RoboForge.Modules.Production.Application.IntegrationEvents;
using RoboForge.Modules.Production.Infrastructure.Database;
using RoboForge.Shared.Hosting;
using RoboForge.Shared.Infrastructure.Outbox;

namespace RoboForge.Modules.Production.Infrastructure;

public static class ProductionModuleServiceRegistration
{
    /// <summary>
    /// Registers everything the Production module needs: its database context (with the outbox), the outbox
    /// publisher and request validators.
    /// </summary>
    public static void AddProductionModule(this IHostApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString(AspireResourceNames.Database);
        Guard.Against.Null(connectionString, message: $"Connection string '{AspireResourceNames.Database}' not found.");

        builder.Services.AddDbContext<ProductionDbContext>(options => options
            .UseNpgsql(connectionString, npgsql =>
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", ProductionDbContext.Schema))
            .AddInterceptors(new WriteIntegrationEventsToOutboxSaveChangesInterceptor(ProductionDomainEventToIntegrationEventMapper.Map)));

        builder.EnrichNpgsqlDbContext<ProductionDbContext>();

        builder.Services.AddScoped<IProductionDbContext>(services => services.GetRequiredService<ProductionDbContext>());

        builder.Services.AddHostedService<PublishOutboxMessagesHostedService<ProductionDbContext>>();

        builder.Services.AddValidatorsFromAssembly(typeof(IProductionDbContext).Assembly, includeInternalTypes: true);
    }
}
