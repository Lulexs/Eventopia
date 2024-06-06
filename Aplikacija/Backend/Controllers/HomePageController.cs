namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HomePageController : ControllerBase
{
    public Context Context { get; set; }

    public HomePageController(Context context)
    {
        Context = context;
    }

    [AllowAnonymous]
    [HttpGet("getHighlights")]
    public async Task<IActionResult> GetHighlights()
    {
        var highlights = await Context.Dogadjaji.Include(x => x.Rezervacije)
                                                .Where(x => x.VideoLink != null)
                                                .OrderByDescending(x => x.Rezervacije!.Count)
                                                .Select(x => new
                                                {
                                                    x.ID,
                                                    EmbedSrc = x.VideoLink,
                                                    x.Vreme
                                                })
                                                .Take(8)
                                                .ToListAsync();

        var highlightsFiltered = highlights.Where(x => (x.Vreme + TimeSpan.FromDays(7)) < DateTime.Now)
                                                    .Select(x => new
                                                    {
                                                        x.ID,
                                                        x.EmbedSrc
                                                    })
                                                    .ToList();

        if (highlightsFiltered.Count == 0)
            highlightsFiltered = highlights.Where(x => (x.Vreme + TimeSpan.FromDays(14)) < DateTime.Now)
                                                    .Select(x => new
                                                    {
                                                        x.ID,
                                                        x.EmbedSrc
                                                    })
                                                    .ToList();

        return Ok(highlights);
    }

    [AllowAnonymous]
    [HttpGet("getAllEvents/{currentPage}")]
    public async Task<ActionResult<List<FullEventDto>>> GetAllEvents(int currentPage)
    {
        var events = await Context.Dogadjaji
                                    .Include(x => x.Organizator)
                                    .Include(x => x.Tagovi)
                                    .Include(x => x.RezervacijaProstora)
                                    .Where(x => x.Status == StatusDogadjaja.Active)
                                    .OrderBy(x => x.Vreme)
                                    .Skip(currentPage * 10)
                                    .Take(10)
                                    .Select(x => new FullEventDto
                                    {
                                        ID = x.ID,
                                        Naziv = x.Naziv,
                                        Slika = x.Slika,
                                        Datum = x.Vreme.ToString("dd.MM.yyyy."),
                                        Vreme = x.Vreme.ToString("HH:mm"),
                                        Lokacija = $"{x.RezervacijaProstora!.Prostor!.Grad}, {x.RezervacijaProstora.Prostor.Drzava}",
                                        OrganizatorID = x.Organizator!.Id.ToString(),
                                        Organizator = $"{x.Organizator!.Ime} {x.Organizator!.Prezime}",
                                    })
                                    .ToListAsync();
        if (events.Count == 0)
            return NotFound();

        return Ok(events);
    }

    [AllowAnonymous]
    [HttpGet("getFilteredEvents/{currentPage}")]
    public async Task<ActionResult<List<FullEventDto>>> GetFilteredEvents(int currentPage, [FromQuery] string? location, [FromQuery] string? search, [FromQuery] string? date, [FromQuery] string[]? tags)
    {
        DateTime dateParsed = DateTime.Now;
        bool dateFilter = !String.IsNullOrEmpty(date) && DateTime.TryParseExact(date, "dd.MM.yyyy.", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateParsed);

        var events = await Context.Dogadjaji
                                    .Include(x => x.Organizator)
                                    .Include(x => x.Tagovi)
                                    .Include(x => x.RezervacijaProstora)
                                    .Where(x => x.Status == StatusDogadjaja.Active
                                                && (String.IsNullOrEmpty(location) ? true : (x.RezervacijaProstora!.Prostor!.Grad + ", " + x.RezervacijaProstora.Prostor.Drzava) == location)
                                                && (String.IsNullOrEmpty(search) ? true : x.Naziv.ToLower().Contains(search.ToLower()))
                                                && (!dateFilter ? true : x.Vreme.Date == dateParsed.Date)
                                                && (tags!.Length == 0 ? true : x.Tagovi!.Any(y => tags.Contains(y.TagName))))
                                    .OrderBy(x => x.Vreme)
                                    .Skip(currentPage * 10)
                                    .Take(10)
                                    .Select(x => new FullEventDto
                                    {
                                        ID = x.ID,
                                        Naziv = x.Naziv,
                                        Slika = x.Slika,
                                        Datum = x.Vreme.ToString("dd.MM.yyyy."),
                                        Vreme = x.Vreme.ToString("HH:mm"),
                                        Lokacija = $"{x.RezervacijaProstora!.Prostor!.Grad}, {x.RezervacijaProstora.Prostor.Drzava}",
                                        OrganizatorID = x.Organizator!.Id.ToString(),
                                        Organizator = $"{x.Organizator!.Ime} {x.Organizator!.Prezime}",
                                    })
                                    .ToListAsync();

        return Ok(events);
    }

    [AllowAnonymous]
    [HttpGet("getLocations")]
    public async Task<IActionResult> GetLocations()
    {
        var locations = await Context.Dogadjaji
                                        .Include(x => x.RezervacijaProstora)
                                        .ThenInclude(x => x!.Prostor)
                                        .Select(x => new
                                        {
                                            x.RezervacijaProstora!.Prostor!.Grad,
                                            x.RezervacijaProstora!.Prostor!.Drzava
                                        })
                                        .Distinct()
                                        .OrderBy(x => x.Drzava)
                                        .ThenBy(x => x.Grad)
                                        .ToListAsync();

        var locationStrings = locations
                                .Select(x => $"{x.Grad}, {x.Drzava}")
                                .ToList();

        return Ok(locationStrings);
    }

    [AllowAnonymous]
    [HttpGet("getOrganizers")]
    public async Task<IActionResult> GetOrganizers()
    {
        var organizers = await Context.Dogadjaji
                                    .Include(x => x.Organizator)
                                    .Select(x => $"{x.Organizator!.Ime} {x.Organizator!.Prezime}")
                                    .Distinct()
                                    .ToListAsync();

        return Ok(organizers);
    }

    [AllowAnonymous]
    [HttpGet("getTags")]
    public async Task<IActionResult> GetTags()
    {
        var tags = await Context.Tagovi
                                    .OrderBy(x => x.TagName)
                                    .Select(x => $"{x.TagName}")
                                    .ToListAsync();

        return Ok(tags);
    }


}