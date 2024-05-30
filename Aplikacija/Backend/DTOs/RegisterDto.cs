namespace Backend.DTOs;

public class RegisterDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public required string UserType { get; set; }

    [Required]
    public required string Ime { get; set; }

    [Required]
    public required string Prezime { get; set; }

    [Required]
    public required string Telefon { get; set; }

    [Required]
    public DateTime DatumRodjenja { get; set; }

    public string? Slika { get; set; }

    public string? Adresa { get; set; }

    public string? Grad { get; set; }

    // TODO: Identification image

}