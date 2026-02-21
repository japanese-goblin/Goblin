using Goblin.DataAccess.Configurations;
using Goblin.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Goblin.DataAccess;

public class BotDbContext(DbContextOptions<BotDbContext> options) : DbContext(options)
{
    public DbSet<BotUser> BotUsers { get; set; }

    public DbSet<Remind> Reminds { get; set; }

    public DbSet<CronJob> CronJobs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new BotUserConfiguration());
        modelBuilder.ApplyConfiguration(new RemindConfiguration());
        modelBuilder.ApplyConfiguration(new CronJobConfiguration());
    }
}