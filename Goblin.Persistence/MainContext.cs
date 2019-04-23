using System.Linq;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goblin.Persistence
{
    public class MainContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Remind> Reminds { get; set; }
        public DbSet<RecurringJob> Jobs { get; set; }

        public MainContext(DbContextOptions<MainContext> options) : base(options) { }

        public User[] GetUsers()
        {
            return Users.AsNoTracking().ToArray();
        }

        public long[] GetAdmins()
        {
            return Users.AsNoTracking().Where(x => x.IsAdmin).Select(x => x.Vk).ToArray();
        }

        public User[] GetWeatherUsers()
        {
            return Users.AsNoTracking().Where(x => x.Weather && x.City != "").ToArray();
        }

        public User[] GetScheduleUsers()
        {
            return Users.AsNoTracking().Where(x => x.Schedule && x.Group != 0).ToArray();
        }
    }
}