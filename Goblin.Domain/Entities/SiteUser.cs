using Microsoft.AspNetCore.Identity;

namespace Goblin.Domain.Entities
{
    public class SiteUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}