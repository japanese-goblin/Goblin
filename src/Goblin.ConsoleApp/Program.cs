using Goblin.Application.Core;
using Goblin.Application.Vk;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddYamlFile("appsettings.yaml", false)
    .AddYamlFile($"appsettings.{builder.Environment.EnvironmentName}.yaml", true)
    .AddYamlFile("appsettings.Secrets.yaml", true)
    .AddEnvironmentVariables();

builder.Services.AddSerilog(p => p.ReadFrom.Configuration(builder.Configuration));
builder.Services.AddDataAccessLayer(builder.Configuration);
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddMemoryCache();
builder.Services.AddVkLongPollingLayer();

var host = builder.Build();
await using(var scope = host.Services.CreateAsyncScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();
    await dbContext.Database.MigrateAsync();
}

await host.RunAsync();
