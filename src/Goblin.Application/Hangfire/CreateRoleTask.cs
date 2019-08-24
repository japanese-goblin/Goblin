using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace Goblin.Application.Hangfire
{
    public class CreateRoleTask
    {
        private readonly RoleManager<IdentityRole> _manager;

        public CreateRoleTask(RoleManager<IdentityRole> manager)
        {
            _manager = manager;
        }

        public async Task CreateRoles()
        {
            const string roleName = "Admin";
            if(await _manager.RoleExistsAsync(roleName))
            {
                return;
            }

            Log.Information("Создание {0} роли", roleName);
            var role = new IdentityRole(roleName);
            await _manager.CreateAsync(role);
        }
    }
}