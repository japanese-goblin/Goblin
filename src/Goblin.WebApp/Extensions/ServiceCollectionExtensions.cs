using Goblin.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Goblin.WebApp.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<IdentityUsersDbContext>()
                    .AddDefaultUI();

            services.AddAuthentication()
                    .AddGitHub("github", options =>
                    {
                        options.ClientId = config["Github:ClientId"];
                        options.ClientSecret = config["Github:ClientSecret"];
                        
                        options.Scope.Add("user:email");
                    });
        }
    }
}