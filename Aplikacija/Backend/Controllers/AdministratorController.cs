namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AdministratorController : ControllerBase
{
    private readonly UserManager<Korisnik> _userManager;
    public Context Context { get; set; }

    public AdministratorController(UserManager<Korisnik> userManager, Context context)
    {
        _userManager = userManager;
        Context = context;
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getUsersWithRoles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var korisnici = await _userManager.Users.OrderBy(korisnik => korisnik.Ime)
                                                .ThenBy(korisnik => korisnik.Prezime)
                                                .Select(korisnik => new
                                                {
                                                    korisnik.Id,
                                                    korisnik.Ime,
                                                    korisnik.Prezime,
                                                    korisnik.Email,
                                                    Role = korisnik.UserRole
                                                }).ToListAsync();
        return Ok(korisnici);
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getUsersWithBans")]
    public async Task<ActionResult<List<KorisnikSaZabranamaDto>>> GetUsersWithBans()
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
                foreach (var zabrana in korisnik.KorisnikZabrane)
                {
                    korisniciSaZabranama.Add(new KorisnikSaZabranamaDto
                    {
                        KorisnikId = korisnik.Id.ToString(),
                        ZabranaId = zabrana.Id,
                        Ime = korisnik.Ime,
                        Prezime = korisnik.Prezime,
                        DatumOd = zabrana.DatumOd,
                        DatumDo = zabrana.DatumDo,
                        Razlog = zabrana.Razlog
                    });
                }
            }
            else
            {
                korisniciSaZabranama.Add(new KorisnikSaZabranamaDto
                {
                    KorisnikId = korisnik.Id.ToString(),
                    Ime = korisnik.Ime,
                    Prezime = korisnik.Prezime
                });
            }
        }

        return Ok(korisniciSaZabranama);

    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpPost("banUser")]
    public async Task<ActionResult> BanUser([FromBody] BanUserDto banUserDto)
    {
        var admin = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var korisnik = await _userManager.FindByIdAsync(banUserDto.KorisnikId);

        if (korisnik == null)
            return BadRequest("User does not exist.");

        if (korisnik.KorisnikZabrane!.Any(x => x.DatumDo > DateTime.Now))
            return BadRequest("User is already banned.");

        Zabrana zabrana = new()
        {
            DatumOd = banUserDto.DatumOd,
            DatumDo = banUserDto.DatumDo,
            Razlog = banUserDto.Razlog,
            Korisnik = korisnik
        };

        await Context.Zabrane.AddAsync(zabrana);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpDelete("unbanUser/{zabranaId}")]
    public async Task<ActionResult> UnbanUser(int zabranaId)
    {
        var zabrana = await Context.Zabrane.FirstOrDefaultAsync(x => x.Id == zabranaId);

        if (zabrana == null)
            return BadRequest("Ban does not exist.");

        Context.Zabrane.Remove(zabrana);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpDelete("deleteEvent/{id}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        var dogadjaj = await Context.Dogadjaji.FirstOrDefaultAsync(x => x.ID == id);

        if (dogadjaj == null)
            return BadRequest("Event does not exist.");

        Context.Dogadjaji.Remove(dogadjaj);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getAllEvents")]
    public async Task<ActionResult<List<DogadjajDto>>> GetAllEvents()
    {
        var dogadjaji = await Context.Dogadjaji
                                     .Select(x => new DogadjajDto
                                     {
                                         ID = x.ID,
                                         Naziv = x.Naziv,
                                         Datum = x.Vreme,
                                         Slika = x.Slika
                                     }).ToListAsync();

        return Ok(dogadjaji);
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpDelete("deleteComment/{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        var ocena = await Context.Ocene.FirstOrDefaultAsync(x => x.ID == id);

        if (ocena == null)
            return BadRequest("Comment does not exist.");

        Context.Ocene.Remove(ocena);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getAllComments")]
    public async Task<ActionResult> GetAllComments()
    {
        var ocene = await Context.Ocene
                               .Select(x => new
                               {
                                   x.ID,
                                   Comment = x.Komentar,
                               }).ToListAsync();

        return Ok(ocene);
    }

}