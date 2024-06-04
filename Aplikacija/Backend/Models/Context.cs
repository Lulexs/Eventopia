namespace Backend.Models;

public class Context : IdentityDbContext<Korisnik, AppRole, Guid,
                                IdentityUserClaim<Guid>, AppUserRole, IdentityUserLogin<Guid>,
                                IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
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
    public DbSet<SurfaceDimension> SurfaceDimensions { get; set; }
    public DbSet<Zabrana> Zabrane { get; set; }
    public DbSet<UserTag> UserTags { get; set; }
    public DbSet<KorisnikTagovi> KorisnikTagovi { get; set; }
    public DbSet<VisitorRank> VisitorRanks { get; set; }

    public Context(DbContextOptions options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Korisnik>()
            .HasOne(korisnik => korisnik.UserRole)
            .WithOne(role => role.Korisnik);

        builder.Entity<AppRole>()
            .HasMany(role => role.UserRoles)
            .WithOne(korisnik => korisnik.Role)
            .HasForeignKey(korisnikRoles => korisnikRoles.RoleId);

        builder.Entity<Prostor>()
            .HasMany(prostor => prostor.PlanoviProstora)
            .WithOne(planProstora => planProstora.Prostor)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PlanProstora>()
            .HasMany(planProstora => planProstora.DraggableItems)
            .WithOne(draggableItem => draggableItem.PlanProstora)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PlanProstora>()
            .HasMany(planProstora => planProstora.Lines)
            .WithOne(line => line.PlanProstora)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PlanProstora>()
            .HasOne(planProstora => planProstora.SurfaceDimension)
            .WithOne(surfaceDimension => surfaceDimension.PlanProstora)
            .OnDelete(DeleteBehavior.Cascade);

    }
}