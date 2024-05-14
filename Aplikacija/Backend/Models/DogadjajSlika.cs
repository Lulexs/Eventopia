namespace Backend.Models;

public class DogadjajSlika
{
    [Key]
    public int ID { get; set; }
    // Slika se pamti kao lokacija na disku. Treba da se uradi hash-ovanje ID-ja slike
    public required string Slika { get; set; }
    public required Dogadjaj Dogadjaj { get; set; }
}