using System.Threading.Tasks;
using Goblin.DataAccess;
using Goblin.Domain.Abstractions;
using Goblin.Domain.Entities;
using Goblin.Narfu.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Goblin.BackgroundJobs.Jobs
{
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
            await CheckAndRemoveUserGroups<VkBotUser>();
            await CheckAndRemoveUserGroups<TgBotUser>();
        }

        public async Task CheckAndRemoveUserGroups<T>() where T : BotUser
        {
            var users = await _context.Set<T>().ToArrayAsync();
            foreach(var user in users)
            {
                if(_narfuApi.Students.IsCorrectGroup(user.NarfuGroup))
                {
                    continue;
                }

                user.SetNarfuGroup(0);
                user.SetHasSchedule(false);
            }

            await _context.SaveChangesAsync();
        }
    }
}