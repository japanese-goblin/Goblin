using Goblin.DataAccess.Configurations;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goblin.DataAccess
{
    public class BotDbContext : DbContext
    {
        public DbSet<BotUser> BotUsers { get; set; }
        public DbSet<Subscribe> Subscribes { get; set; }
        public DbSet<Remind> Reminds { get; set; }

        public DbSet<CronJob> CronJobs { get; set; }

        public BotDbContext(DbContextOptions<BotDbContext> options)
                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new BotUserConfiguration());
            modelBuilder.ApplyConfiguration(new SubscribeConfiguration());
            modelBuilder.ApplyConfiguration(new RemindConfiguration());
            modelBuilder.ApplyConfiguration(new CronJobConfiguration());
        }
    }
}