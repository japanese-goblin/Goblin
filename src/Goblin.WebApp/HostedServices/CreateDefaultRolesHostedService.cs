using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Goblin.WebApp.HostedServices
{
    public class CreateDefaultRolesHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public CreateDefaultRolesHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var manager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            
            const string roleName = "Admin";
            if(await manager.RoleExistsAsync(roleName))
            {
                return;
            }

            Log.ForContext<CreateDefaultRolesHostedService>().Debug("Создание {0} роли", roleName);
            var role = new IdentityRole(roleName);
            await manager.CreateAsync(role);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}