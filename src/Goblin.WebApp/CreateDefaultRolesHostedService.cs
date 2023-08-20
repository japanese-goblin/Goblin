using Microsoft.AspNetCore.Identity;

namespace Goblin.WebApp;

public class CreateDefaultRolesHostedService : IHostedService
{
    private readonly string[] _roles =
    {
        RoleNames.User,
        RoleNames.Admin
    };

    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<CreateDefaultRolesHostedService> _logger;

    public CreateDefaultRolesHostedService(IServiceProvider serviceProvider, ILogger<CreateDefaultRolesHostedService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
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

            _logger.LogInformation("Создание {RoleName} роли", role);
            await manager.CreateAsync(new IdentityRole(role));
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}