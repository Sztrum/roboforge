using Microsoft.EntityFrameworkCore;

namespace RoboForge.Infrastructure.Data;

/// <summary>
/// Brings the local database schema up to date by applying pending EF Core migrations.
/// </summary>
public sealed class ApplyPendingMigrationsDatabaseInitialiser(ApplicationDbContext context)
{
    public Task InitialiseAsync() => context.Database.MigrateAsync();
}
