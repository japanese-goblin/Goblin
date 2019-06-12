using Goblin.Persistence;
using Goblin.WebUI.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Goblin.WebUI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                                      .InitializeDatabase<BotDbContext>()
                                      .InitializeDatabase<ApplicationDbContext>()
                                      .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>();
        }
    }
}