using Goblin.BackgroundJobs.Jobs;
using Hangfire;

namespace Goblin.WebApp;

public static class ApplicationExtensions
{
    public static void UseHangfireJobs(this WebApplication app)
    {
        BackgroundJob.Enqueue<StartupTasks>(x => x.ConfigureHangfire());
        BackgroundJob.Enqueue<StartupTasks>(x => x.RemoveInactiveUsersFromVk());
        BackgroundJob.Enqueue<ResetUsersGroups>(x => x.Execute());
    }
}