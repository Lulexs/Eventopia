namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class SpaceController : ControllerBase
{

    private readonly UserManager<Korisnik> _userManager;
    public Context Context { get; set; }

    public SpaceController(Context context, UserManager<Korisnik> userManager)
    {
        Context = context;
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireSpaceOwnerRole")]
    [HttpPost("addSpace")]
    public async Task<IActionResult> AddSpace([FromBody] SpaceDto prostorDto)
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        Prostor prostor = new Prostor
        {
            Grad = prostorDto.Grad,
            Drzava = prostorDto.Drzava,
            Adresa = prostorDto.Adresa,
            Latitude = prostorDto.Latitude,
            Longitude = prostorDto.Longitude,
            VlasnikProstora = korisnik
        };

        if (prostorDto.DraggableItems == null || prostorDto.DraggableItems?.Count == 0)
        {
            return BadRequest("Space must have at least one draggable item.");
        }

        PlanProstora planProstora = new PlanProstora
        {
            Prostor = prostor
        };

        List<DraggableItem> draggableItems = new List<DraggableItem>();

        foreach (DraggableItemDto draggableItemDto in prostorDto.DraggableItems!)
        {
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

        List<Line> lines = new List<Line>();

        foreach (LineDto lineDto in prostorDto.Lines!)
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
            Width = prostorDto.SurfaceDimension!.Width,
            Height = prostorDto.SurfaceDimension!.Height,
            PlanProstora = planProstora
        };

        await Context.SurfaceDimensions.AddAsync(surfaceDimension);
        await Context.Prostori.AddAsync(prostor);
        await Context.PlanoviProstora.AddAsync(planProstora);
        await Context.SaveChangesAsync();

        return Ok();
    }

    [Authorize(Policy = "RequireSpaceOwnerRole")]
    [HttpGet("getOwnerSpaces")]
    public async Task<IActionResult> GetOwnerSpaces()
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        if (korisnik == null)
        {
            return BadRequest("User not found.");
        }

        var prostori = await Context.Prostori.Where(x => x.VlasnikProstora == korisnik).ToListAsync();

        return Ok(prostori?.Select(x => new
        {
            Id = x.ID,
            City = x.Grad,
            Country = x.Drzava,
            Address = x.Adresa,
            Latitude = x.Latitude,
            Longitude = x.Longitude
        }).OrderBy(x => x.Id));

    }

    [Authorize(Policy = "RequireSpaceOwnerRole")]
    [HttpDelete("deleteSpace/{id}")]
    public async Task<IActionResult> DeleteSpace([FromRoute] int id)
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var prostori = await Context.Prostori.Where(x => x.VlasnikProstora == korisnik).ToListAsync();

        if (korisnik!.VlasnikProstori?.Find(x => x.ID == id) == null)
        {
            return BadRequest("You are not the owner of this space.");
        }

        var prostor = await Context.Prostori.Where(x => x.ID == id)
                                            .Include(x => x.Rezervacije)
                                            .FirstOrDefaultAsync();

        if (prostor == null)
        {
            return BadRequest("Space not found.");
        }

        bool hasConfirmedReservations = false;

        prostor.Rezervacije?.ForEach(x =>
        {
            if (x.Status == StatusRezervacije.Confirmed)
                hasConfirmedReservations = true;
        });

        if (hasConfirmedReservations)
            return BadRequest("Space has confirmed reservations.");

        Context.Prostori.Remove(prostor);
        await Context.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Policy = "RequireSpaceOwnerRole")]
    [HttpPut("respondToSpaceReservation/{id}/{response}")]
    public async Task<IActionResult> RespondToSpaceReservation([FromRoute] int id, [FromRoute] string response)
    {

        var rezervacija = await Context.RezervacijeProstora.FirstOrDefaultAsync(x => x.ID == id);

        if (rezervacija == null)
        {
            return BadRequest("Reservation not found.");
        }

        if (rezervacija.Status != StatusRezervacije.WaitingConfirmation)
        {
            return BadRequest("Reservation is not pending.");
        }

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var prostori = await Context.Prostori.Where(x => x.VlasnikProstora == korisnik).ToListAsync();

        if (korisnik!.VlasnikProstori?.Find(x => x.ID == rezervacija!.Prostor!.ID) == null)
        {
            return BadRequest("You are not the owner of this space.");
        }

        if (response == "accept")
        {
            rezervacija.Status = StatusRezervacije.Confirmed;
        }
        else if (response == "reject")
        {
            rezervacija.Status = StatusRezervacije.Rejected;
            rezervacija.Dogadjaj!.Status = StatusDogadjaja.SpaceRejected;
        }
        else
        {
            return BadRequest("Invalid response.");
        }

        Context.RezervacijeProstora.Update(rezervacija);
        await Context.SaveChangesAsync();
        return Ok();
    }

    [Authorize(Policy = "RequireSpaceOwnerRole")]
    [HttpGet("getSpacesReservations")]
    public async Task<ActionResult<List<ProstorRezervacijeDto>>> GetSpacesReservations()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var prostori = await Context.Prostori.Where(x => x.VlasnikProstora == korisnik)
                                            .Include(x => x.Rezervacije!)
                                            .ThenInclude(x => x.Dogadjaj)
                                            .ToListAsync();

        List<ProstorRezervacijeDto> rezervacije = new List<ProstorRezervacijeDto>();

        foreach (var prostor in prostori)
        {
            foreach (var rezervacija in prostor.Rezervacije!)
            {
                string statusRezervacije = rezervacija.Status.ToString();
                if (statusRezervacije == "Rejected")
                    continue;
                string status = rezervacija.Dogadjaj!.Vreme < DateTime.Now ? "Finished" : statusRezervacije;
                rezervacije.Add(new ProstorRezervacijeDto
                {
                    ID = rezervacija.ID,
                    NazivDogadjaja = rezervacija!.Dogadjaj!.Naziv,
                    Adresa = prostor.Adresa,
                    VremeOd = rezervacija.VremeOd,
                    VremeDo = rezervacija.VremeDo,
                    Status = status
                });
            }
        }

        return Ok(rezervacije.OrderByDescending(x => x.ID));
    }

    [Authorize(Policy = "RequireSpaceOwnerRole")]
    [HttpGet("getStatistics")]
    public async Task<ActionResult<List<ProstorRezervacijeDto>>> GetStatistics()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var banned = await UserUtils.IsBanned(korisnik!, Context);

        if (banned != null)
        {
            return Unauthorized($"You are banned from the platform until {banned.DatumDo.ToShortDateString()}. Reason: {banned.Razlog}");
        }

        var prostori = await Context.Prostori.Where(x => x.VlasnikProstora == korisnik)
                                            .Include(x => x.Rezervacije!)
                                            .ThenInclude(x => x.Dogadjaj)
                                            .ToListAsync();

        int rentableSpaces = prostori.Count;
        int totalRents = 0;

        foreach (var prostor in prostori)
        {
            foreach (var rezervacija in prostor.Rezervacije!)
            {
                if (rezervacija.Status == StatusRezervacije.Confirmed)
                    totalRents++;
            }
        }

        return Ok(new SpaceOwnerStatisticsDto
        {
            RentableSpaces = rentableSpaces,
            TotalRents = totalRents
        });
    }

}