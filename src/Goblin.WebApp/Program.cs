using System.Globalization;
using System.Net.Mime;
using FastEndpoints;
using FastEndpoints.Swagger;
using Goblin.Application.Core;
using Goblin.Application.Telegram;
using Goblin.Application.Vk;
using Goblin.DataAccess;
using Goblin.WebApp;
using Goblin.WebApp.Extensions;
using Goblin.WebApp.HostedServices;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;

SetDefaultLocale();

var builder = WebApplication.CreateBuilder(args);
if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Host.ConfigureLogging(config =>
       {
           config.ClearProviders();
       })
       .UseSerilog((hostingContext, loggerConfiguration) =>
       {
           loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
           if(!hostingContext.HostingEnvironment.IsDevelopment())
           {
               loggerConfiguration
                       .WriteTo.Sentry(o =>
                       {
                           o.MinimumBreadcrumbLevel = LogEventLevel.Information;
                           o.MinimumEventLevel = LogEventLevel.Warning;
                           o.Dsn = hostingContext.Configuration["Sentry:Dsn"];
                           o.Environment = hostingContext.HostingEnvironment.EnvironmentName;
                       });
           }
       });
builder.Services.AddHttpLogging(x =>
{
    x.LoggingFields = HttpLoggingFields.All;
});

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddVkLayer(builder.Configuration);
builder.Services.AddTelegramLayer(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});
builder.Services.AddHangfireServer(x =>
{
    x.WorkerCount = 4;
});
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });
builder.Services.AddSwaggerDoc(shortSchemaNames: true);
builder.Services.AddFastEndpoints();
builder.Services.AddHostedService<AddHangfireJobsHostedService>();

var app = builder.Build();
app.MigrateDatabase<BotDbContext>();
app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = MediaTypeNames.Application.Json;

        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError
        };

        if(builder.Environment.IsProduction())
        {
            problemDetails.Title = "Внутренняя ошибка сервера";
            problemDetails.Detail = "Возникла непредвиденная ошибка. Пожалуйста, попробуйте позже.";
        }
        else
        {
            var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>()!;
            problemDetails.Title = exceptionHandlerFeature.Error.Message;
            problemDetails.Detail = exceptionHandlerFeature.Error.StackTrace;
        }

        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});
app.UseFastEndpoints(c =>
{
    c.RoutingOptions = ro => ro.Prefix = "api";
    c.ErrorResponseBuilder = (failures, _) => failures.Select(x => x.ErrorMessage);
});
if(app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3(s => s.ConfigureDefaults());
}

app.Run();

void SetDefaultLocale()
{
    var culture = new CultureInfo("ru-RU");
    CultureInfo.CurrentCulture = culture;
    CultureInfo.CurrentUICulture = culture;
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}