using Goblin.WebApp.HostedServices;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;

namespace Goblin.WebApp.Extensions;

internal static class WebApplicationBuilderExtensions
{
    public static WebApplicationBuilder RegisterLogging(this WebApplicationBuilder builder)
    {
        builder.Configuration.AddYamlFile("appsettings.yaml", false)
               .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true)
               .AddYamlFile("appsettings.secrets.yaml", true)
               .AddEnvironmentVariables();

        builder.Services.AddSerilog(p =>
        {
            p.ReadFrom.Configuration(builder.Configuration);
        });

        builder.Services.AddHttpLogging(x =>
        {
            x.LoggingFields = HttpLoggingFields.All;
        });

        return builder;
    }

    public static WebApplicationBuilder RegisterHangfire(this WebApplicationBuilder builder)
    {
        builder.Services.AddHangfire(config =>
        {
            config.UseMemoryStorage();
        });
        builder.Services.AddHangfireServer(x =>
        {
            x.WorkerCount = 4;
        });
        GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
        builder.Services.AddHostedService<AddHangfireJobsHostedService>();

        return builder;
    }

    public static WebApplicationBuilder RegisterSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddOpenApi();

        return builder;
    }
}