namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HostController : ControllerBase
{
    private readonly UserManager<Korisnik> _userManager;
    public Context Context { get; set; }
    public HostController(Context context, UserManager<Korisnik> userManager)
    {
        Context = context;
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpPost("createEvent")]
    public async Task<ActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        string dateTimeString = $"{createEventDto.Datum} {createEventDto.Vreme}";
        DateTime dateTime;
        if (DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {

            List<Tag> tags = new List<Tag>();
            foreach (var tag in createEventDto.Tags)
            {
                Tag existingTag = await Context.Tagovi.FirstOrDefaultAsync(x => x.TagName == tag);
                if (existingTag != null)
                {
                    tags.Add(existingTag);
                }
                else
                {
                    Tag newTag = new Tag
                    {
                        TagName = tag,
                        Dogadjaji = new List<Dogadjaj>()
                    };
                    tags.Add(newTag);
                }
            }
            var dogadjaj = new Dogadjaj
            {
                Naziv = createEventDto.Naziv,
                Opis = createEventDto.Opis,
                Slika = createEventDto.Slika,
                Vreme = dateTime,
                Organizator = korisnik,
                VideoLink = createEventDto.Video,
                Status = StatusDogadjaja.Active, // nisam siguran sta bi trebalo da se prenese
                Tagovi = tags,
                //ne znam za rezervacijaProstora i Rezervacija treba
            };

            foreach (var tag in dogadjaj.Tagovi)
            {
                Tag existingTag = await Context.Tagovi.FirstOrDefaultAsync(x => x.TagName == tag.TagName);
                if (existingTag != null)
                {
                    existingTag.Dogadjaji.Add(dogadjaj);
                }
                else
                {
                    Context.Tagovi.Update(tag);
                }

            }

            await Context.Dogadjaji.AddAsync(dogadjaj);
        }
        else
        {
            return BadRequest("Invalid date and time format.");
        }


        return Ok();
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getAvailableSpaces/{date}/{time}/{location}/{capacity}")]
    public async Task<ActionResult> GetAvailableSpaces(string date, string time, string location, int capacity)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        string dateTimeString = $"{date} {time}";
        DateTime dateTime = DateTime.Now;
        if (!DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {
            return BadRequest("Invalid date and time format.");
        }

        var spaces = await Context.Prostori.Include(x => x.Rezervacije)
                                           .Include(x => x.PlanoviProstora!)
                                           .ThenInclude(x => x.DraggableItems)
                                           .Include(x => x.PlanoviProstora!)
                                           .ThenInclude(x => x.Dogadjaj)
                                           .Where(x => x.Rezervacije!.All(y => !(dateTime >= y.VremeOd && dateTime <= y.VremeDo))
                                                    && (location != "null" ? x.Grad + ", " + x.Drzava == location : true)
                                                    && (capacity != -1 ? x.PlanoviProstora!
                                                                        .Where(y => y.Dogadjaj == null)
                                                                        .FirstOrDefault()!
                                                                        .Kapacitet >= capacity : true)
                                           )
                                           .Select(x => new SpaceBasicDto
                                           {
                                               ID = x.ID,
                                               Grad = x.Grad,
                                               Drzava = x.Drzava,
                                               Adresa = x.Adresa,
                                               Kapacitet = x.PlanoviProstora!.Where(y => y.Dogadjaj == null).FirstOrDefault()!.Kapacitet
                                           })
                                           .ToListAsync();

        return Ok(spaces);
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getSpacePlan/{spaceId}")]
    public async Task<ActionResult> GetSpacePlan(int spaceId)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        SpaceDto? spacePlan = await Context.PlanoviProstora.Include(x => x.Prostor)
                                               .Include(x => x.DraggableItems)
                                               .Include(x => x.Lines)
                                               .Include(x => x.SurfaceDimension)
                                               .Where(x => x.Prostor!.ID == spaceId && x.Dogadjaj == null)
                                               .Select(x => new SpaceDto
                                               {
                                                   ID = x.ID,
                                                   DraggableItems = x.DraggableItems!.Select(y => new DraggableItemDto
                                                   {
                                                       ID = y.ID,
                                                       FrontID = y.FrontID,
                                                       Tip = y.Tip.ToString().ToLower(),
                                                       Top = y.Top,
                                                       Left = y.Left,
                                                       Height = y.Height,
                                                       HeightFactor = y.HeightFactor,
                                                       BrojMesta = y.BrojMesta,
                                                       Reserved = y.Reserved,
                                                       Price = y.Price
                                                   }).ToList(),
                                                   Lines = x.Lines!.Select(y => new LineDto
                                                   {
                                                       X1 = y.X1,
                                                       Y1 = y.Y1,
                                                       X2 = y.X2,
                                                       Y2 = y.Y2
                                                   }).ToList(),
                                                   SurfaceDimension = new SurfaceDimensionDto
                                                   {
                                                       Width = x.SurfaceDimension!.Width,
                                                       Height = x.SurfaceDimension!.Height
                                                   }
                                               })
                                               .FirstOrDefaultAsync();

        return Ok(spacePlan);
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpPost("manageEvent")]
    public async Task<ActionResult> ManageEvent([FromBody] CreateEventDto createEventDto)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        return Ok();
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpDelete("deleteEvent/{id}")]
    public async Task<ActionResult> DeleteEvent([FromRoute] int id)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        var dogadjaj = await Context.Dogadjaji.FirstOrDefaultAsync(x => x.Organizator == korisnik && x.ID == id);
        if (dogadjaj == null)
        {
            return NotFound("Event not found.");
        }

        Context.Dogadjaji.Remove(dogadjaj);
        await Context.SaveChangesAsync();
        return Ok();
    }


    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getIncomingEvents")]
    public async Task<ActionResult> GetIncomingEvents()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        var dogadjaji = await Context.Dogadjaji.Where(x => x.Organizator == korisnik && x.Vreme > DateTime.Now).ToListAsync();

        if (dogadjaji == null)
        {
            return NotFound("No incoming events found for the given host.");
        }

        List<EventForListDto> events = new List<EventForListDto>();
        foreach (var dogadjaj in dogadjaji)
        {
            events.Add(new EventForListDto
            {
                Naziv = dogadjaj.Naziv,
                Slika = dogadjaj.Slika,
                Datum = dogadjaj.Vreme.ToString("dd.MM.yyyy."),
            });
        }


        return Ok(events);
    }




    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getPastEvents")]
    public async Task<ActionResult> GetPastEvents()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        var dogadjaji = await Context.Dogadjaji.Where(x => x.Organizator == korisnik && x.Vreme < DateTime.Now).ToListAsync();
        if (dogadjaji == null)
        {
            return NotFound("No past events found for the given host.");
        }

        if (dogadjaji == null)
        {
            return NotFound("No past events found for the given host.");
        }
        List<EventForListDto> events = new List<EventForListDto>();

        foreach (var dogadjaj in dogadjaji)
        {
            events.Add(new EventForListDto
            {
                Naziv = dogadjaj.Naziv,
                Slika = dogadjaj.Slika,
                Datum = dogadjaj.Vreme.ToString("dd.MM.yyyy."),
            });
        }

        //kako zelim DTO da bude
        return Ok(events);
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getReviewsForEvent/{id}")]
    public async Task<ActionResult> GetReviewsForEvent([FromRoute] int id)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        var dogadjaj = await Context.Dogadjaji.FirstOrDefaultAsync(x => x.Organizator == korisnik && x.ID == id);
        if (dogadjaj == null)
        {
            return NotFound("Event not found.");
        }

        List<Ocena> ocene = await Context.Ocene.Where(x => x.Dogadjaj == dogadjaj).ToListAsync();
        List<ReviewDto> reviews = new List<ReviewDto>();

        //json namesti za ReviewDto
        foreach (var oc in ocene)
        {
            reviews.Add(new ReviewDto
            {
                Vrednost = oc.Vrednost,
                Komentar = oc.Komentar,
                Korisnik = $"{oc.Korisnik.Ime} {oc.Korisnik.Prezime}",
                VremeKomentara = oc.VremeKomentara,

            });
        }

        return Ok(reviews);
    }

}
