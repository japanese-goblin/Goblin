using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Goblin.DataAccess.ContextFactories
{
    public class IdentityUsersDbContextFactory : IDesignTimeDbContextFactory<IdentityUsersDbContext>
    {
        public IdentityUsersDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory() +
                           string.Format("{0}..{0}Goblin.WebApp", Path.DirectorySeparatorChar);

            var configuration = new ConfigurationBuilder()
                                .SetBasePath(basePath)
                                .AddJsonFile("appsettings.Development.json")
                                .AddEnvironmentVariables()
                                .Build();

            var builder = new DbContextOptionsBuilder<IdentityUsersDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            builder.UseNpgsql(connectionString);
            return new IdentityUsersDbContext(builder.Options);
        }
    }
}