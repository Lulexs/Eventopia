using Microsoft.AspNetCore.Identity;

namespace Backend.Models;


//nasledjivanjem IdentityUser klase pokriva PasswordHash, PasswordSalt, Username i id
//takodje, ne brinite o ID polju, ono je vec uvedeno u IdentityUser klasi zajedno sa svojstvom [Key],
// ovo int u <int> je samo zadavanje tipa id-a
public class Korisnik : IdentityUser<int>
{
    // TODO
    public AppUserRole UserRole { get; set; }

}