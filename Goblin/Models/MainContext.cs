using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Goblin.Models
{
    public class MainContext : DbContext
    {
        public DbSet<Person> Persons { get; set; }

        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {

        }
    }
}