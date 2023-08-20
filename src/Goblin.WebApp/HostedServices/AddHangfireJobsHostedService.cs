using Goblin.BackgroundJobs.Jobs;
using Hangfire;

namespace Goblin.WebApp.HostedServices;

public class AddHangfireJobsHostedService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        BackgroundJob.Enqueue<StartupTasks>(x => x.ConfigureHangfire());
        BackgroundJob.Enqueue<StartupTasks>(x => x.RemoveInactiveUsersFromVk());
        BackgroundJob.Enqueue<ResetUsersGroups>(x => x.Execute());
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}