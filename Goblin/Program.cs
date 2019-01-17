using FluentScheduler;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Globalization;

namespace Goblin
{
    public class Program
    {
        public static DateTime StartDate;

        public static void Main(string[] args)
        {
            Setup();
            BuildWebHost(args).Run();
        }

        private static void Setup()
        {
            StartDate = DateTime.Now;

            JobManager.Initialize(new ScheduledTasks());

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