using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Goblin.WebApp.Extensions
{
    public static class WebHostExtensions
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
                var logger = provider.GetService<ILogger>();
                try
                {
                    var context = provider.GetRequiredService<T>();

                    context.Database.Migrate();
                }
                catch(Exception ex)
                {
                    logger?.LogError(ex, "Database migrate error");
                    throw;
                }
            }

            return host;
        }
    }
}