using Microsoft.EntityFrameworkCore;

namespace RoboForge.Web.Database;

public static class ApplyPendingMigrationsWebApplicationExtensions
{
    /// <summary>Brings a module's database schema up to date by applying its pending EF Core migrations.</summary>
    public static async Task ApplyPendingMigrationsAsync<TDbContext>(this WebApplication app)
        where TDbContext : DbContext
    {
        using var scope = app.Services.CreateScope();

        var dbContext = scope.ServiceProvider.GetRequiredService<TDbContext>();

        await dbContext.Database.MigrateAsync();
    }
}
