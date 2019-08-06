using Goblin.Application;
using Goblin.Application.Abstractions;
using Goblin.Application.Commands;
using Goblin.DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace Goblin.WebApp
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<BotDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<IdentityUsersDbContext>(options => options.UseSqlServer(connectionString));

            services.AddSingleton<IVkApi, VkApi>(x =>
            {
                var token = Configuration["Vk:AccessToken"];
                var api = new VkApi();
                api.Authorize(new ApiAuthParams
                {
                    AccessToken = token
                });
                return api;
            });

            services.AddScoped<IBotCommand, RandomCommand>();
            services.AddScoped<CommandsService>();
            services.AddScoped<CallbackHandler>();
            
            services.AddDefaultIdentity<IdentityUser>()
                    .AddDefaultUI(UIFramework.Bootstrap4)
                    .AddEntityFrameworkStores<IdentityUsersDbContext>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if(env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();
        }
    }
}