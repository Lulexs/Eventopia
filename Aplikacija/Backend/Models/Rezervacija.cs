namespace Backend.Models;

public class Rezervacija
{
    [Key]
    public int ID { get; set; }
    public DateTime VremeRezervacije { get; set; }
    public int BrojMesta { get; set; }
    [ForeignKey("StoFK")]
    public DraggableItem? Sto { get; set; }
    public required Dogadjaj Dogadjaj { get; set; }
    public required Korisnik Korisnik { get; set; }
}