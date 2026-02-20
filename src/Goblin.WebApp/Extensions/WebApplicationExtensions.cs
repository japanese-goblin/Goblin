using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Extensions;

internal static class WebApplicationExtensions
{
    public static void MigrateDatabase<T>(this WebApplication app) where T : DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}