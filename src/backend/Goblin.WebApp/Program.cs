using System.Globalization;
using System.Net.Mime;
using FastEndpoints;
using FastEndpoints.Swagger;
using Goblin.Application.Core;
using Goblin.Application.Telegram;
using Goblin.Application.Vk;
using Goblin.DataAccess;
using Goblin.WebApp;
using Goblin.WebApp.Filters;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

SetDefaultLocale();

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
                      policy =>
                      {
                          var cors = builder.Configuration.GetSection("CORS").Get<string[]>();
                          policy.WithOrigins(cors)
                                .AllowCredentials()
                                .AllowAnyHeader()
                                .AllowAnyMethod();
                          policy.SetIsOriginAllowed(origin => true);
                      });
});

builder.Services.AddHostedService<CreateDefaultRolesHostedService>();
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
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
       .AddEntityFrameworkStores<IdentityUsersDbContext>();
builder.Services.AddAuthentication()
       .AddGitHub("github", options =>
       {
           options.ClientId = builder.Configuration["Github:ClientId"];
           options.ClientSecret = builder.Configuration["Github:ClientSecret"];

           options.Scope.Add("user:email");
       });
builder.Services.ConfigureApplicationCookie(o =>
{
    o.Events.OnRedirectToLogin = ctx =>
    {
        ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return Task.CompletedTask;
    };
});
builder.Services.AddHostedService<CreateDefaultRolesHostedService>();
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();
MigrateDatabase<BotDbContext>(app);
MigrateDatabase<IdentityUsersDbContext>(app);
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
app.UseResponseCompression();
app.UseCors(corsName);
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/Admin/HangFire", new DashboardOptions
{
    Authorization = new[] { new AuthFilter() },
    AppPath = "/",
    StatsPollingInterval = 10000,
    DisplayStorageConnectionString = false
});
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

app.UseHangfireJobs();

app.Run();

void MigrateDatabase<T>(WebApplication application) where T : DbContext
{
    using var scope = application.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<T>();
    context.Database.Migrate();
}

void SetDefaultLocale()
{
    var culture = new CultureInfo("ru-RU");
    CultureInfo.CurrentCulture = culture;
    CultureInfo.CurrentUICulture = culture;
    CultureInfo.DefaultThreadCurrentCulture = culture;
    CultureInfo.DefaultThreadCurrentUICulture = culture;
}