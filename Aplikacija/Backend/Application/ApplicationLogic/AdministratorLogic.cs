using Backend.ApplicationLogic.Exceptions;

namespace Backend.ApplicationLogic;

public class AdministratorLogic
{
    private readonly UserManager<Korisnik> _userManager;
    public Context Context { get; set; }

    public AdministratorLogic(UserManager<Korisnik> userManager, Context context)
    {
        _userManager = userManager;
        Context = context;
    }

    public async Task<List<KorisnikSaZabranamaDto>> GetUsersWithBans()
    {
        var korisnici = await _userManager.Users.OrderBy(korisnik => korisnik.Ime)
                                        .ThenBy(korisnik => korisnik.Prezime)
                                        .Include(x => x.KorisnikZabrane)
                                        .Include(x => x.UserRole)
                                        .ThenInclude(x => x!.Role)
                                        .Where(x => x.UserRole!.Role!.Name != "Admin")
                                        .ToListAsync();

        List<KorisnikSaZabranamaDto> korisniciSaZabranama = new();

        foreach (var korisnik in korisnici)
        {
            if (korisnik.KorisnikZabrane?.Count > 0)
            {
                Zabrana? zabrana = korisnik.KorisnikZabrane.OrderBy(x => x.DatumOd).LastOrDefault();
                korisniciSaZabranama.Add(new KorisnikSaZabranamaDto
                {
                    KorisnikId = korisnik.Id.ToString(),
                    ZabranaId = zabrana!.Id,
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Avatar = korisnik.SlikaProfila,
                    Role = korisnik.UserRole!.Role!.Name,
                    DatumOd = zabrana.DatumOd,
                    DatumDo = zabrana.DatumDo,
                    Razlog = zabrana.Razlog
                });
            }
            else
            {
                korisniciSaZabranama.Add(new KorisnikSaZabranamaDto
                {
                    KorisnikId = korisnik.Id.ToString(),
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime,
                    Avatar = korisnik.SlikaProfila,
                    Role = korisnik.UserRole!.Role!.Name,
                });
            }
        }
        return korisniciSaZabranama;
    }

    public async Task<int> BanUser(BanUserDto banUserDto)
    {
        var korisnik = await _userManager.Users.Include(x => x.KorisnikZabrane).Where(x => x.Id.ToString() == banUserDto.KorisnikId).FirstOrDefaultAsync();

        if (korisnik == null)
            throw new UserNotFoundException("User does not exist.");

        if (korisnik.KorisnikZabrane!.Any(x => x.DatumDo > DateTime.Now))
            throw new AlreadyBannedException("User is already banned.");

        Zabrana zabrana = new()
        {
            DatumOd = banUserDto.DatumOd,
            DatumDo = DateTime.Parse(banUserDto.DatumDo),
            Razlog = banUserDto.Razlog,
            Korisnik = korisnik
        };

        await Context.Zabrane.AddAsync(zabrana);
        await Context.SaveChangesAsync();
        return zabrana.Id;
    }

    public async Task UnbanUser(int zabranaId)
    {
        var zabrana = await Context.Zabrane.FirstOrDefaultAsync(x => x.Id == zabranaId);

        if (zabrana == null)
            throw new BanNotFoundException("Ban does not exist");

        Context.Zabrane.Remove(zabrana);
        await Context.SaveChangesAsync();
    }

    public async Task DeleteEvent(int id)
    {
        var dogadjaj = await Context.Dogadjaji.Include(x => x.RezervacijaProstora).FirstOrDefaultAsync(x => x.ID == id);

        if (dogadjaj == null)
            throw new EventNotFoundException("Event does not exist");

        Context.Dogadjaji.Remove(dogadjaj);
        Context.RezervacijeProstora.Remove(dogadjaj.RezervacijaProstora!);
        await Context.SaveChangesAsync();
    }

    public async Task<List<DogadjajDto>> GetAllEvents()
    {
        var dogadjaji = await Context.Dogadjaji
                                .OrderBy(x => x.Vreme)
                                .Where(x => x.Status == StatusDogadjaja.Active)
                                .Select(x => new DogadjajDto
                                {
                                    ID = x.ID,
                                    Naziv = x.Naziv,
                                    Datum = x.Vreme,
                                    Slika = x.Slika
                                }).ToListAsync();

        return dogadjaji;
    }

    public async Task DeleteComment(int id)
    {
        var ocena = await Context.Ocene.FirstOrDefaultAsync(x => x.ID == id);

        if (ocena == null)
            throw new CommentNotFoundException("Comment does not exist");

        Context.Ocene.Remove(ocena);
        await Context.SaveChangesAsync();
    }

    public async Task<List<ReturnOcenaDto>> GetAllComments()
    {
        var ocene = await Context.Ocene
                                .OrderByDescending(x => x.VremeKomentara)
                                .Select(x => new ReturnOcenaDto
                                {
                                    Id = x.ID,
                                    Komentar = x.Komentar,
                                }).ToListAsync();
        return ocene;
    }

}