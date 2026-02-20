using System.Reflection;
using Goblin.Application.Core.Options;
using Goblin.Application.Core.Services;
using Goblin.Narfu;
using Goblin.OpenWeatherMap;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin.Application.Core;

public static class SetupExtensions
{
    private const string MailingSettingsPath = "Mailing";

    public static void AddApplication(this IServiceCollection services)
    {
        AddBotFeatures(services);
        AddOptions(services);
        AddAdditions(services);
        services.AddNarfuApi()
                .AddOpenWeatherMapApi();
    }

    private static void AddAdditions(IServiceCollection services)
    {
        services.AddSingleton<IScheduleService, ScheduleService>();
        services.AddSingleton<IWeatherService, WeatherService>();
    }

    private static void AddBotFeatures(IServiceCollection services)
    {
        services.RegisterAllTypes<ITextCommand>([typeof(SetupExtensions).Assembly], ServiceLifetime.Scoped);
        services.RegisterAllTypes<IKeyboardCommand>([typeof(SetupExtensions).Assembly], ServiceLifetime.Scoped);

        services.AddScoped<CommandsService>();
    }

    private static void AddOptions(IServiceCollection services)
    {
        services.AddOptions<MailingOptions>()
                .BindConfiguration(MailingSettingsPath)
                .ValidateDataAnnotations()
                .ValidateOnStart();
    }

    private static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies,
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