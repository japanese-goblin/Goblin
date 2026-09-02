using Goblin.DataAccess;
using Goblin.Narfu.Abstractions;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Goblin.BackgroundJobs.Jobs;

internal class ResetUsersGroupsJob(BotDbContext context, INarfuApi narfuApi) : IJob
{
    public static readonly JobKey JobKey = new("V1", "reset-users-groups");

    public async Task Execute(IJobExecutionContext context1)
    {
        var users = await context.BotUsers.Where(p => p.NarfuGroup.HasValue).ToListAsync();
        foreach (var user in users)
        {
            if (!user.NarfuGroup.HasValue)
            {
                continue;
            }

            var group = narfuApi.Students.GetGroupByRealId(user.NarfuGroup.Value);
            if (group is not null)
            {
                continue;
            }

            user.SetHasSchedule(false);
            user.SetNarfuGroup(null);
        }

        await context.SaveChangesAsync();
    }
}
