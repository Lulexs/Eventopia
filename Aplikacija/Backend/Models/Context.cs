namespace Backend.Models;

public class Context : DbContext
{

    public DbSet<Dogadjaj> Dogadjaji { get; set; }
    public DbSet<DraggableItem> DraggableItems { get; set; }
    public DbSet<Korisnik> Korisnici { get; set; }
    public DbSet<Line> Lines { get; set; }
    public DbSet<Ocena> Ocene { get; set; }
    public DbSet<PlanProstora> PlanoviProstora { get; set; }
    public DbSet<Prostor> Prostori { get; set; }
    public DbSet<Rezervacija> Rezervacije { get; set; }
    public DbSet<RezervacijaProstora> RezervacijeProstora { get; set; }
    public DbSet<Tag> Tagovi { get; set; }

    public Context(DbContextOptions options) : base(options)
    {

    }
}