using Microsoft.AspNetCore.Identity;
using YourNamespace;

namespace Backend.Models
{
    public class AppUserRole : IdentityUserRole<int>
    {
        public required Korisnik Korisnik { get; set; }

        public required AppRole Role { get; set; }

    }
}