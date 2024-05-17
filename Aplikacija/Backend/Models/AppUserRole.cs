using Microsoft.AspNetCore.Identity;


namespace Backend.Models;

    public class AppUserRole : IdentityUserRole<Guid>
    {
        public required Korisnik Korisnik { get; set; }

        public required AppRole Role { get; set; }

    }
