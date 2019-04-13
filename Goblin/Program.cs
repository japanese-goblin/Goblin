using System.Globalization;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Goblin
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Setup();
            BuildWebHost(args).Run();
        }

        private static void Setup()
        {
            var culture = new CultureInfo("ru-RU");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>()
                          .Build();
        }
    }
}