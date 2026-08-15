using Goblin.BackgroundJobs.Options;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Goblin.BackgroundJobs;

public static class SetupExtensions
{
    public static IServiceCollection AddBackgroundJobs(this IServiceCollection services)
    {
        services.AddQuartz();
        services.ConfigureOptions<ConfigureQuartzOptions>();
        return services;
    }
}
