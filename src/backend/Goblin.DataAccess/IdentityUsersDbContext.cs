using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Goblin.DataAccess;

public class IdentityUsersDbContext : IdentityDbContext
{
    public IdentityUsersDbContext(DbContextOptions<IdentityUsersDbContext> options)
            : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}