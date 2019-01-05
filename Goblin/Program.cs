using FluentScheduler;
using Goblin.Bot;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using Vk;

namespace Goblin
{
    public class Program
    {
        public static DateTime StartDate;

        public static void Main(string[] args)
        {
            StartDate = DateTime.Now;
            Task.Factory.StartNew(async () => await Messages.Send(Settings.Developers, "БОТ ЗАПУСКАЕТСЯ..."));
            //TODO: а это точно тут должно быть?
            JobManager.Initialize(new ScheduledTasks());
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