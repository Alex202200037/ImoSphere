using Microsoft.AspNetCore.Identity;

namespace ImoSphere.Models
{
    public class UserWithRolesViewModel
    {
        public IdentityUser User { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}