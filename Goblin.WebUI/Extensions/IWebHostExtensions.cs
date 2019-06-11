using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin.WebUI.Extensions
{
    public static class IWebHostExtensions
    {
        /// <summary>
        ///     Initialize Database
        /// </summary>
        /// <typeparam name="T">Database Context</typeparam>
        /// <param name="host">Web Host</param>
        public static IWebHost InitializeDatabase<T>(this IWebHost host)
            where T : DbContext
        {
            using(var serviceScope = host.Services.CreateScope())
            {
                var provider = serviceScope.ServiceProvider;
                var context = provider.GetRequiredService<T>();

                context.Database.Migrate();

                //TODO: logger
            }

            return host;
        }
    }
}