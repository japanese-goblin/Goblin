using Goblin.BackgroundJobs;
using Microsoft.AspNetCore.HttpLogging;
using Quartz.AspNetCore;
using Serilog;

namespace Goblin.WebApp.Extensions;

internal static class WebApplicationBuilderExtensions
{
    extension(WebApplicationBuilder builder)
    {
        public WebApplicationBuilder RegisterLogging()
        {
            builder.Configuration
                .AddYamlFile("appsettings.yaml", false)
                   .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true)
                   .AddYamlFile("appsettings.Secrets.yaml", true)
                   .AddEnvironmentVariables();

            builder.Services.AddSerilog(p =>
            {
                p.ReadFrom.Configuration(builder.Configuration);
            });

            builder.Services.AddHttpLogging(p =>
            {
                p.LoggingFields = HttpLoggingFields.All;
            });

            return builder;
        }

        public WebApplicationBuilder RegisterQuartz()
        {
            builder.Services.AddBackgroundJobs();

            builder.Services.AddQuartzServer(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;
            });

            return builder;
        }

        public WebApplicationBuilder RegisterSwagger()
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi();

            return builder;
        }
    }
}