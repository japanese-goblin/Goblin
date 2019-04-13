using Goblin.Bot.Commands;
using Goblin.Bot.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin
{
    public static class ServiceProviderExtensions
    {
        public static void AddBotCommands(this IServiceCollection services)
        {
            services.AddScoped<ICommand, AddRemind>();
            services.AddScoped<ICommand, Debug>();
            services.AddScoped<ICommand, Exams>();
            services.AddScoped<ICommand, FindTeacher>();
            services.AddScoped<ICommand, GetReminds>();
            services.AddScoped<ICommand, Keyboard>();
            services.AddScoped<ICommand, Quote>();
            services.AddScoped<ICommand, Random>();
            services.AddScoped<ICommand, Schedule>();
            services.AddScoped<ICommand, SendAdmin>();
            services.AddScoped<ICommand, SetCity>();
            services.AddScoped<ICommand, SetGroup>();
            services.AddScoped<ICommand, SetMailing>();
            services.AddScoped<ICommand, TeacherSchedule>();
            services.AddScoped<ICommand, UnsetMailing>();
            services.AddScoped<ICommand, Weather>();
            services.AddScoped<ICommand, Help>();
            services.AddScoped<ICommand, MuteErrors>();
        }
    }
}