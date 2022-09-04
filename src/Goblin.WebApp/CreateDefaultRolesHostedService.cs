using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Goblin.WebApp;

public class CreateDefaultRolesHostedService : IHostedService
{
    private readonly string[] _roles =
    {
        RoleNames.User,
        RoleNames.Admin
    };

    private readonly IServiceProvider _serviceProvider;

    public CreateDefaultRolesHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var manager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        foreach(var role in _roles)
        {
            if(await manager.RoleExistsAsync(role))
            {
                return;
            }

            Log.ForContext<CreateDefaultRolesHostedService>().Information("Создание {RoleName} роли", role);
            await manager.CreateAsync(new IdentityRole(role));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}