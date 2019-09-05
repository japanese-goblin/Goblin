using System.Globalization;
using Goblin.DataAccess;
using Goblin.WebApp.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Goblin.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetDefaultLocale();

            CreateWebHostBuilder(args).Build()
                                      .InitializeDatabase<IdentityUsersDbContext>()
                                      .InitializeDatabase<BotDbContext>()
                                      .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .ConfigureLogging(config =>
                          {
                              config.ClearProviders();
                          })
                          .UseSerilog((hostingContext, loggerConfiguration) =>
                          {
                              loggerConfiguration
                                      .ReadFrom.Configuration(hostingContext.Configuration);
                          })
                          .UseStartup<Startup>();
        }

        private static void SetDefaultLocale()
        {
            var culture = new CultureInfo("ru-RU");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }
    }
}