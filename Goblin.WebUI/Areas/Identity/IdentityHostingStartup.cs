using Microsoft.AspNetCore.Hosting;

[assembly: HostingStartup(typeof(Goblin.WebUI.Areas.Identity.IdentityHostingStartup))]
namespace Goblin.WebUI.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {
            builder.ConfigureServices((context, services) => {
            });
        }
    }
}