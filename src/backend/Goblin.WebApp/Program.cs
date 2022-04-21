using System.Net.Mime;
using FastEndpoints;
using FastEndpoints.Swagger;
using Goblin.Application.Core;
using Goblin.Application.Telegram;
using Goblin.Application.Vk;
using Goblin.DataAccess;
using Goblin.WebApp;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Events;

const string corsName = "frontend";

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
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsName,
                      policy  =>
                      {
                          var cors = builder.Configuration.GetSection("CORS").Get<string[]>();
                          policy.WithOrigins(cors);
                      });
});

// builder.Services.AddHostedService<MigrationHostedService>();
// builder.Services.AddHostedService<CreateDefaultRolesHostedService>();

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddVkLayer(builder.Configuration);
builder.Services.AddTelegramLayer(builder.Configuration);
builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});
builder.Services.AddMemoryCache();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddHangfireServer(x =>
{
    x.WorkerCount = 4;
});
builder.Services.AddSwaggerDoc(shortSchemaNames: true);
builder.Services.AddFastEndpoints();
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });


var app = builder.Build();
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

app.UseCors(corsName);
app.UseAuthorization();
app.UseFastEndpoints(c =>
{
    c.RoutingOptions = o => o.Prefix = "api";
    c.ErrorResponseBuilder = (failures, _) => failures.Select(x => x.ErrorMessage);
});
if(app.Environment.IsDevelopment())
{
    app.UseOpenApi();
    app.UseSwaggerUi3(s => s.ConfigureDefaults());
}
app.Run();