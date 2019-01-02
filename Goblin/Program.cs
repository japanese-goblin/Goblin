using System;
using System.Threading.Tasks;
using FluentScheduler;
using Goblin.Helpers;
using Goblin.Vk;
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
            Task.Factory.StartNew(async () => await VkMethods.SendMessage(VkMethods.DevelopersID, "БОТ ЗАПУСКАЕТСЯ..."));
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