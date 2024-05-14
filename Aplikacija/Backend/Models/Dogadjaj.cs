namespace Backend.Models;

public class Dogadjaj
{
    [Key]
    public int ID { get; set; }
    [MaxLength(80)]
    public required string Naziv { get; set; }
    // Proveriti pri dodavanju da li je vreme u buducnosti
    public DateTime Vreme { get; set; }
    public required string Opis { get; set; }
    // Sta treba da se pamti za ovo?
    public required string Lokacija { get; set; }
    // Active, Passed, Cancelled
    public StatusDogadjaja Status { get; set; }
    public List<DogadjajSlika>? Slike { get; set; }
    // Video se pamti kao link ka YouTube videu
    public string? VideoLink { get; set; }
    public List<Tag>? Tagovi { get; set; }
    public List<Ocena>? Ocene { get; set; }

    // veza sa prostorom
    // rezervacije?

    public Korisnik? Organizator { get; set; }
}