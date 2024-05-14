using Microsoft.AspNetCore.Identity;

namespace YourNamespace
{
    // jedan Role Moze imati vise Usera, 1 user 1 role
    public class AppRole : IdentityRole<int>
    {
        public required ICollection<AppUserRole> UserRoles { get; set; } // jedan Role Moze imati vise Usera, 1 user 1 role
    }
}