using System;
using System.Threading;
using System.Threading.Tasks;
using Goblin.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Goblin.WebApp.HostedServices
{
    public class MigrationHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public MigrationHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var identityContext = scope.ServiceProvider.GetRequiredService<IdentityUsersDbContext>();
            await identityContext.Database.MigrateAsync(cancellationToken);


            var botContext = scope.ServiceProvider.GetRequiredService<BotDbContext>();
            await botContext.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}