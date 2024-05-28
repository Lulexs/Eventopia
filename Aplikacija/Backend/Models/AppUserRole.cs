using Microsoft.AspNetCore.Identity;


namespace Backend.Models;

    public class AppUserRole : IdentityUserRole<Guid>
    {
        
        [ForeignKey("UserRole")]
        public Korisnik? Korisnik { get; set; }


        
        public AppRole? Role { get; set; }

    }
