using System.Globalization;
using System.Net.Mime;
using Goblin.Application.Core;
using Goblin.Application.Telegram;
using Goblin.Application.Telegram.Options;
using Goblin.Application.Vk;
using Goblin.Application.Vk.Options;
using Goblin.DataAccess;
using Goblin.WebApp.Extensions;
using Goblin.WebApp.HostedServices;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Newtonsoft.Json;
using Serilog;
using Serilog.Events;
using Telegram.Bot.Types;
using VkNet.Enums.StringEnums;
using VkNet.Model;

SetDefaultLocale();

var builder = WebApplication.CreateBuilder(args);
if(builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
builder.Configuration
       .AddJsonFile("appsettings.Secrets.json", true)
       .AddEnvironmentVariables();

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
builder.Services.AddTelegramLayer();
builder.Services.AddMemoryCache();
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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// builder.Services.AddSwaggerGen(c =>
// {
//     c.SwaggerDoc("v1", new OpenApiInfo
//     {
//         Title = "Japanese Goblin",
//         Version = "v1"
//     });
// });

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
app.MapPost("/api/callback/vk",
            ([FromBody] GroupUpdate requestModel, [FromServices] VkCallbackHandler handler, [FromServices] IOptions<VkOptions> vkOptions) =>
            {
                if(requestModel.Type.Value == GroupUpdateType.Confirmation)
                {
                    return Results.Ok(vkOptions.Value.ConfirmationCode);
                }

                BackgroundJob.Enqueue(() => handler.Handle(requestModel));

                return Results.Ok("ok");
            });
app.MapPost("/api/callback/tg/{SecretKey}", async (string secretKey, HttpRequest httpRequest,
                                                   [FromServices] IOptions<TelegramOptions> options,
                                                   [FromServices] TelegramCallbackHandler handler) =>
{
    if(!options.Value.SecretKey.Equals(secretKey))
    {
        //TODO: logging
        return Results.NotFound();
    }

    var rawRequestBody = await new StreamReader(httpRequest.Body).ReadToEndAsync();
    var request = JsonConvert.DeserializeObject<Update>(rawRequestBody)!;

    BackgroundJob.Enqueue(() => handler.Handle(request));

    return Results.Ok();
});

if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", $"{builder.Environment.ApplicationName} v1");
    });
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