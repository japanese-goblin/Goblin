using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.WebApp.Extensions;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Goblin.WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                                      .InitializeDatabase<IdentityUsersDbContext>()
                                      .InitializeDatabase<BotDbContext>()
                                      .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
                WebHost.CreateDefaultBuilder(args)
                       .UseStartup<Startup>();
    }
}