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
                    .AddVkontakte(options =>
                    {
                        options.ApiVersion = "5.95";
                        options.ClientId = config["VkAuth:AppId"];
                        options.ClientSecret = config["VkAuth:SecretKey"];
                        options.Scope.Add("email");
                    });
        }
    }
}