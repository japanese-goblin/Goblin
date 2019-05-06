using System.Linq;
using Goblin.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<SiteUser>
    {
        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<Remind> Reminds { get; set; }
        public DbSet<RecurringJob> Jobs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public BotUser[] GetUsers()
        {
            return BotUsers.AsNoTracking().ToArray();
        }

        public long[] GetAdmins()
        {
            return BotUsers.AsNoTracking().Where(x => x.IsAdmin).Select(x => x.Vk).ToArray();
        }

        public BotUser[] GetWeatherUsers()
        {
            return BotUsers.AsNoTracking().Where(x => x.Weather && x.City != "").ToArray();
        }

        public BotUser[] GetScheduleUsers()
        {
            return BotUsers.AsNoTracking().Where(x => x.Schedule && x.Group != 0).ToArray();
        }
    }
}