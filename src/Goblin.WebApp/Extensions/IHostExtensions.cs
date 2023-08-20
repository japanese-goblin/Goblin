using Microsoft.EntityFrameworkCore;

namespace Goblin.WebApp.Extensions;

public static class IHostExtensions
{
    public static void MigrateDatabase<T>(this IHost host) where T : DbContext
    {
        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        context.Database.Migrate();
    }
}