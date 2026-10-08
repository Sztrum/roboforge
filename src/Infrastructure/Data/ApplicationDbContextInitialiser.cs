using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace RoboForge.Infrastructure.Data;

public static class InitialiserExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initialiser = scope.ServiceProvider.GetRequiredService<ApplicationDbContextInitialiser>();

        await initialiser.InitialiseAsync();
    }
}

/// <summary>
/// Brings the local database schema up to date by applying pending EF Core migrations.
/// </summary>
public sealed class ApplicationDbContextInitialiser(ApplicationDbContext context)
{
    public Task InitialiseAsync() => context.Database.MigrateAsync();
}
