using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Goblin.Models
{
    public class MainContext : DbContext
    {
        public DbSet<Person> Persones { get; set; }

        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {

        }
    }
}