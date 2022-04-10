using FastEndpoints;
using Goblin.Application.Core;
using Goblin.Application.Telegram;
using Goblin.Application.Vk;
using Goblin.DataAccess;
using Goblin.WebApp;
using Hangfire;
using Hangfire.MemoryStorage;
using Microsoft.AspNetCore.HttpLogging;
using Serilog;
using Serilog.Events;

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

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
builder.Services.AddFastEndpoints();
// builder.Services.AddHostedService<MigrationHostedService>();
// builder.Services.AddHostedService<CreateDefaultRolesHostedService>();

builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddVkLayer(builder.Configuration);
builder.Services.AddTelegramLayer(builder.Configuration);
builder.Services.AddHangfire(config => { config.UseMemoryStorage(); });
builder.Services.AddMemoryCache();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddHangfireServer(x =>
{
    x.WorkerCount = 4;
});
GlobalJobFilters.Filters.Add(new AutomaticRetryAttribute { Attempts = 0 });


var app = builder.Build();

// Configure the HTTP request pipeline.
// if(app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }

app.UseAuthorization();
app.UseFastEndpoints();
app.Run();