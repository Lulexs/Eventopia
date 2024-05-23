using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs;


/*Ime = registerDto.Ime,
            Prezime = registerDto.Prezime,
            Email = registerDto.Email,
            UserName = registerDto.Username,
            Telefon = registerDto.Telefon,
            DatumRodjenja = registerDto.DatumRodjenja,
            SlikaProfila = registerDto.Slika;*/
public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string  Email { get; set; }

    [Required]
    [RegularExpression("(?=.*\\d)(?=.*[a-z])(?=.*[A-Z]).{4,8}$", ErrorMessage = "Password must be complex")]
    public required string Password { get; set; }

    [Required]
    public required string Ime { get; set; }
    [Required]
    public required string Prezime { get; set; }

    [Required]
    public required string Username { get; set; }

    [Required]
    public required string Telefon { get; set; }

    [Required]
    public required string Slika { get; set; }

    [Required   ]   
    public DateTime DatumRodjenja { get; set; } 


    //TODO: USERROLE obrati paznju na tip ovde je string
    [Required]
    public required string UserRole { get; set; }
}