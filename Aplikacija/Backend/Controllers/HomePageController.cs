using Castle.Components.DictionaryAdapter;

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
        var highlights = await Context.Dogadjaji.Where(x => x.VideoLink != null)
                                                .Select(x => new
                                                {
                                                    x.ID,
                                                    EmbedSrc = x.VideoLink,
                                                    x.Vreme
                                                }).ToListAsync();

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
    [HttpGet("getAllEvents")]
    public async Task<ActionResult<List<FullEventDto>>> GetAllEvents()
    {
        var events = await Context.Dogadjaji
                                    .Include(x => x.Organizator)
                                    .Include(x => x.Tagovi)
                                    .Include(x => x.RezervacijaProstora)
                                    .Select(x => new FullEventDto
                                    {
                                        ID = x.ID,
                                        Naziv = x.Naziv,
                                        Slika = x.Slika,
                                        Datum = x.Vreme.ToString("dd.MM.yyyy"),
                                        Vreme = x.Vreme.ToString("HH:mm"),
                                        Lokacija = $"{x.RezervacijaProstora!.Prostor!.Grad}, {x.RezervacijaProstora.Prostor.Drzava}",
                                        OrganizatorID = x.Organizator!.Id.ToString(),
                                        Organizator = $"{x.Organizator!.Ime} {x.Organizator!.Prezime}",
                                    })
                                    .ToListAsync();

        return Ok(events);
    }

    [AllowAnonymous]
    [HttpGet("getFilteredEvents")]
    public async Task<ActionResult<List<FullEventDto>>> GetFilteredEvents([FromQuery] string? location, [FromQuery] string? search, [FromQuery] string? date, [FromQuery] string[]? tags)
    {
        DateTime dateParsed = DateTime.Now;
        bool dateFilter = !String.IsNullOrEmpty(date) && DateTime.TryParse(date, out dateParsed);

        var events = await Context.Dogadjaji
                                    .Include(x => x.Organizator)
                                    .Include(x => x.Tagovi)
                                    .Include(x => x.RezervacijaProstora)
                                    .Where(x => (String.IsNullOrEmpty(location) ? true : (x.RezervacijaProstora!.Prostor!.Grad + ", " + x.RezervacijaProstora.Prostor.Drzava) == location)
                                                && (String.IsNullOrEmpty(search) ? true : x.Naziv.ToLower().Contains(search.ToLower()))
                                                && (!dateFilter ? true : x.Vreme.Date == dateParsed.Date)
                                                && (tags!.Length == 0 ? true : x.Tagovi!.Any(y => tags.Contains(y.TagName))))
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
                                    .Select(x => $"{x.RezervacijaProstora!.Prostor!.Grad}, {x.RezervacijaProstora.Prostor.Drzava}")
                                    .Distinct()
                                    .ToListAsync();

        return Ok(locations);
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
                                    .Select(x => $"{x.TagName}")
                                    .ToListAsync();

        return Ok(tags);
    }


}