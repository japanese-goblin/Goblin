using Goblin.DataAccess.Configurations;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goblin.DataAccess
{
    public class BotDbContext : DbContext
    {
        public DbSet<TgBotUser> TgBotUsers { get; set; }
        public DbSet<VkBotUser> VkBotUsers { get; set; }
        public DbSet<Remind> Reminds { get; set; }

        public DbSet<CronJob> CronJobs { get; set; }

        public BotDbContext(DbContextOptions<BotDbContext> options)
                : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new VkBotUserConfiguration());
            modelBuilder.ApplyConfiguration(new TgBotUserConfiguration());
            modelBuilder.ApplyConfiguration(new RemindConfiguration());
            modelBuilder.ApplyConfiguration(new CronJobConfiguration());
        }
    }
}