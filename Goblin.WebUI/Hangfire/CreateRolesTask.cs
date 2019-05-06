using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Goblin.WebUI.Hangfire
{
    public class CreateRolesTask
    {
        private readonly RoleManager<IdentityRole> _manager;

        public CreateRolesTask(RoleManager<IdentityRole> manager)
        {
            _manager = manager;
        }

        public async Task CreateRoles()
        {
            var roleName = "Admin";
            if(await _manager.RoleExistsAsync(roleName))
            {
                return;
            }

            var role = new IdentityRole(roleName);
            await _manager.CreateAsync(role);
        }
    }
}