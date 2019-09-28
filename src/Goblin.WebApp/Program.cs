using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Goblin.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetDefaultLocale();
            CreateHostBuilder(args).Build().Run();
        }

        private static void SetDefaultLocale()
        {
            var culture = new CultureInfo("ru-RU");
            CultureInfo.CurrentCulture = culture;
            CultureInfo.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                       .ConfigureLogging(config => { config.ClearProviders(); })
                       .UseSerilog((hostingContext, loggerConfiguration) =>
                       {
                           loggerConfiguration
                                   .ReadFrom.Configuration(hostingContext.Configuration);
                       })
                       .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
        }
    }
}