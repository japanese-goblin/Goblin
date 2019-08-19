using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

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

            var role = new IdentityRole(roleName);
            await _manager.CreateAsync(role);
        }
    }
}