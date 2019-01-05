using FluentScheduler;
using Goblin.Bot;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using Vk;

namespace Goblin
{
    public class Program
    {
        public static DateTime StartDate;

        public static void Main(string[] args)
        {
            StartDate = DateTime.Now;
            // Task.Factory.StartNew(async () => await Messages.Send(DbHelper.GetAdmins(), "БОТ ЗАПУСКАЕТСЯ..."));
            JobManager.Initialize(new ScheduledTasks());
            Api.SetAccessToken(Settings.AccessToken); // TODO
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