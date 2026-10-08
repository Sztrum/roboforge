using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace RoboForge.Infrastructure.Data;

public static class ApplyPendingMigrationsWebApplicationExtensions
{
    public static async Task ApplyPendingMigrationsAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplyPendingMigrationsDatabaseInitialiser>();

        await initialiser.InitialiseAsync();
    }
}
