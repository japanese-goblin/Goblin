using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Goblin.Models
{
    public class MainContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Remind> Reminds { get; set; }

        public MainContext()
        {
        }

        private static string con;

        public MainContext(IConfiguration configuration) //TODO ???
        {
            con = configuration.GetConnectionString("DefaultConnection");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(con);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(b => b.Weather)
                .HasDefaultValue(false)
                .ValueGeneratedNever();

            modelBuilder.Entity<User>()
                .Property(b => b.Schedule)
                .HasDefaultValue(false)
                .ValueGeneratedNever();

            modelBuilder.Entity<User>()
                .Property(b => b.City)
                .IsRequired(false);

            modelBuilder.Entity<User>()
                .Property(b => b.Group)
                .HasDefaultValue(0);

            modelBuilder.Entity<User>()
                .Property(b => b.Schedule)
                .HasDefaultValue(false);

            modelBuilder.Entity<User>()
                .Property(b => b.Weather)
                .HasDefaultValue(false);
        }
    }
}