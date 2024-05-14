namespace Backend.Models;

public class Dogadjaj
{
    [Key]
    public int ID { get; set; }

    [MaxLength(80)]
    public required string Naziv { get; set; }

    // Proveriti pri dodavanju da li je vreme u buducnosti
    public DateTime Vreme { get; set; }

    [MaxLength(250)]
    public required string Opis { get; set; }

    public StatusDogadjaja Status { get; set; }

    // Slika se pamti kao lokacija na disku. Treba da se uradi hash-ovanje ID-ja slike
    public required string Slika { get; set; }

    // Video se pamti kao link ka YouTube videu
    public string? VideoLink { get; set; }

    public List<Tag>? Tagovi { get; set; }
    public List<Ocena>? Ocene { get; set; }

    [ForeignKey("RezervacijaProstoraFK")]
    public required RezervacijaProstora RezervacijaProstora { get; set; }

    public List<Rezervacija>? Rezervacije { get; set; }

    public Korisnik? Organizator { get; set; }
}