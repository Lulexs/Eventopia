using Microsoft.AspNetCore.Identity;

namespace Backend.Models;

    // jedan Role Moze imati vise Usera, 1 user 1 role
    public class AppRole : IdentityRole<Guid>
    {
        public ICollection<AppUserRole>? UserRoles { get; set; } // jedan Role Moze imati vise Usera, 1 user 1 role
    }
