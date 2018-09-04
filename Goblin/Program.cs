using System;
using System.Threading.Tasks;
using Goblin.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Goblin
{
    public class Program
    {
        public static DateTime StartDate;
        public static void Main(string[] args)
        {
            StartDate = DateTime.Now;
            Task.Factory.StartNew(async () => await VkHelper.SendMessage(VkHelper.DevelopersID, "БОТ ЗАПУСКАЕТСЯ..."));
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }
    }
}