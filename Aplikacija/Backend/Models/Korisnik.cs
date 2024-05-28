using Microsoft.AspNetCore.Identity;

namespace Backend.Models;


//nasledjivanjem IdentityUser klase pokriva PasswordHash, PasswordSalt, Username i id
//takodje, ne brinite o ID polju, ono je vec uvedeno u IdentityUser klasi zajedno sa svojstvom [Key],
// ovo int u <int> je samo zadavanje tipa id-a
public class Korisnik : IdentityUser<Guid>
{
    
    

    public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required string Telefon { get; set; }
    public DateTime DatumRodjenja { get; set; }
    public required string SlikaProfila { get; set; }

    
    public AppUserRole? UserRole { get; set; }
    // TODO : ostali atributi Korisnika
}