using System.Linq;
using System.Net.Http;
using System.Reflection;
using Goblin.Application.Core.Abstractions;
using Goblin.Application.Core.Options;
using Goblin.Application.Core.Services;
using Goblin.Narfu;
using Goblin.Narfu.Abstractions;
using Goblin.Narfu.Schedule;
using Goblin.OpenWeatherMap;
using Goblin.OpenWeatherMap.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Goblin.Application.Core;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient();
        AddBotFeatures(services);
        AddOptions(services, configuration);
        AddAdditions(services, configuration);
    }

    private static void AddAdditions(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IOpenWeatherMapApi, OpenWeatherMapApi>(x =>
        {
            return new OpenWeatherMapApi(configuration["OWM:AccessToken"], x.GetService<IHttpClientFactory>(),
                                         x.GetService<ILogger<OpenWeatherMapApi>>());
        });
        services.AddSingleton<INarfuApi, NarfuApi>(x =>
        {
            var link = configuration["Links:NarfuGroups"];
            return new NarfuApi(link, x.GetService<IHttpClientFactory>(), x.GetService<ILogger<TeachersSchedule>>(),
                                x.GetService<ILogger<StudentsSchedule>>());
        });

        services.AddSingleton<IScheduleService, ScheduleService>();
        services.AddSingleton<IWeatherService, WeatherService>();
    }

    private static void AddBotFeatures(IServiceCollection services)
    {
        services.RegisterAllTypes<ITextCommand>(new[] { typeof(DependencyInjection).Assembly }, ServiceLifetime.Scoped);
        services.RegisterAllTypes<IKeyboardCommand>(new[] { typeof(DependencyInjection).Assembly }, ServiceLifetime.Scoped);

        services.AddScoped<CommandsService>();
    }

    private static void AddOptions(IServiceCollection services, IConfiguration config)
    {
        services.Configure<OpenWeatherMapOptions>(config.GetSection("OWM"));
        services.Configure<MailingOptions>(config.GetSection("Mailing"));
        services.Configure<LinksOptions>(config.GetSection("Links"));
    }

    public static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies,
                                           ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var typesFromAssemblies = assemblies.SelectMany(a => a.DefinedTypes
                                                              .Where(x => x.GetInterfaces()
                                                                           .Contains(typeof(T))));
        foreach(var type in typesFromAssemblies)
        {
            services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
        }
    }
}