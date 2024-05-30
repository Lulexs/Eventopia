namespace Backend.Models;

public class Korisnik : IdentityUser<Guid>
{
    public required string Ime { get; set; }
    public required string Prezime { get; set; }
    public required string Telefon { get; set; }
    public DateTime DatumRodjenja { get; set; }
    public string? SlikaProfila { get; set; }
    public string? Adresa { get; set; }
    public string? Grad { get; set; }
    public AppUserRole? UserRole { get; set; }
}