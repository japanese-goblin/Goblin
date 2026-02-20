using Goblin.DataAccess;
using Goblin.Narfu.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Goblin.BackgroundJobs.Jobs;

public class ResetUsersGroups
{
    private readonly BotDbContext _context;
    private readonly INarfuApi _narfuApi;

    public ResetUsersGroups(BotDbContext context, INarfuApi narfuApi)
    {
        _context = context;
        _narfuApi = narfuApi;
    }

    public async Task Execute()
    {
        await CheckAndRemoveUserGroups();
    }

    public async Task CheckAndRemoveUserGroups()
    {
        var users = await _context.BotUsers.Where(p => p.NarfuGroup.HasValue).ToListAsync();
        foreach(var user in users)
        {
            if(!user.NarfuGroup.HasValue)
            {
                continue;
            }

            if(_narfuApi.Students.IsCorrectGroup(user.NarfuGroup.Value))
            {
                continue;
            }

            user.SetNarfuGroup(0);
            user.SetHasSchedule(false);
        }

        await _context.SaveChangesAsync();
    }
}