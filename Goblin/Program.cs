using FluentScheduler;
using Goblin.Bot;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Globalization;
using Vk;

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

            Api.SetAccessToken(Settings.AccessToken); // TODO

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