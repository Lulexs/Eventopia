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
    public async Task<IActionResult> CreateEvent([FromBody] CreateEventDto createEventDto)
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
        if (!DateTime.TryParseExact(dateTimeString, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime))
        {
            return BadRequest("Invalid date and time format.");
        }

        if (dateTime < DateTime.Now)
        {
            return BadRequest("Event date and time must be in the future.");
        }

        var prostor = await Context.Prostori.Include(x => x.PlanoviProstora)
                                            .FirstOrDefaultAsync(x => x.ID == createEventDto.ProstorId);

        if (prostor == null)
        {
            return NotFound("Space not found.");
        }

        List<DraggableItem> draggableItems = new List<DraggableItem>();

        int capacity = 0;

        PlanProstora planProstora = new PlanProstora
        {
            Prostor = prostor
        };

        foreach (DraggableItemDto draggableItemDto in createEventDto.Items!)
        {

            if (draggableItemDto.Tip.ToEnum<TipItema>() == TipItema.Table && draggableItemDto.BrojMesta == 0)
                draggableItemDto.BrojMesta = 4;


            if (draggableItemDto.Tip.ToEnum<TipItema>() == TipItema.Table)
                capacity += draggableItemDto.BrojMesta ?? 0;

            DraggableItem draggableItem = new DraggableItem
            {
                FrontID = draggableItemDto.FrontID,
                Tip = draggableItemDto.Tip.ToEnum<TipItema>(),
                Top = draggableItemDto.Top,
                Left = draggableItemDto.Left,
                Height = draggableItemDto.Height,
                HeightFactor = draggableItemDto.HeightFactor,
                BrojMesta = draggableItemDto.BrojMesta,
                Reserved = draggableItemDto.Reserved,
                Price = draggableItemDto.Price,
                PlanProstora = planProstora
            };

            draggableItems.Add(draggableItem);
        }

        planProstora.DraggableItems = draggableItems;
        planProstora.Kapacitet = capacity;

        List<Line> lines = new List<Line>();

        foreach (LineDto lineDto in createEventDto.Lines!)
        {
            Line line = new Line
            {
                X1 = lineDto.X1,
                Y1 = lineDto.Y1,
                X2 = lineDto.X2,
                Y2 = lineDto.Y2,
                PlanProstora = planProstora
            };
            lines.Add(line);
        }

        planProstora.Lines = lines;

        SurfaceDimension surfaceDimension = new SurfaceDimension
        {
            Width = createEventDto.SurfaceDimension!.Width,
            Height = createEventDto.SurfaceDimension!.Height,
            PlanProstora = planProstora
        };

        await Context.SurfaceDimensions.AddAsync(surfaceDimension);
        await Context.PlanoviProstora.AddAsync(planProstora);

        var rezervacijaProstora = new RezervacijaProstora
        {
            VremeOd = dateTime.AddHours(-12),
            VremeDo = dateTime.AddHours(12),
            Status = StatusRezervacije.WaitingConfirmation,
            Prostor = prostor
        };

        await Context.RezervacijeProstora.AddAsync(rezervacijaProstora);

        var dogadjaj = new Dogadjaj
        {
            Naziv = createEventDto.Naziv,
            Opis = createEventDto.Opis,
            Vreme = dateTime,
            Organizator = korisnik,
            VideoLink = createEventDto.Video,
            Status = StatusDogadjaja.WaitingForSpaceApproval,
            Slika = "",
            PlanProstora = planProstora,
        };

        await Context.Dogadjaji.AddAsync(dogadjaj);

        dogadjaj.RezervacijaProstora = rezervacijaProstora;
        rezervacijaProstora.Dogadjaj = dogadjaj;

        List<Tag> tags = new List<Tag>();
        foreach (var tag in createEventDto.Tags!)
        {
            var existingTag = await Context.Tagovi.Include(x => x.Dogadjaji).FirstOrDefaultAsync(x => x.TagName == tag);
            if (existingTag != null)
            {
                tags.Add(existingTag);
                existingTag.Dogadjaji!.Add(dogadjaj);
            }
            else
            {
                Tag newTag = new Tag
                {
                    TagName = tag,
                    Dogadjaji = new List<Dogadjaj> { dogadjaj }
                };
                await Context.Tagovi.AddAsync(newTag);
                tags.Add(newTag);
            }
        }

        await Context.SaveChangesAsync();
        return Ok(dogadjaj.ID);

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

        var dogadjaj = await Context.Dogadjaji.Include(x => x.Organizator).FirstOrDefaultAsync(x => x.ID == id);

        if (dogadjaj == null)
        {
            return NotFound("Event not found.");
        }

        if (dogadjaj.Organizator != korisnik)
        {
            return Unauthorized("You are not the host of this event.");
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
                Korisnik = $"{oc.Korisnik!.Ime} {oc.Korisnik.Prezime}",
                VremeKomentara = oc.VremeKomentara,

            });
        }

        return Ok(reviews);
    }

}
