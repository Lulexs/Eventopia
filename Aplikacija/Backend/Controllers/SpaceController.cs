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
    public async Task<IActionResult> AddSpace([FromBody] ProstorDto prostorDto)
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

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
        }));

    }

    [Authorize(Policy = "RequireSpaceOwnerRole")]
    [HttpDelete("deleteSpace/{id}")]
    public async Task<IActionResult> DeleteSpace([FromRoute] int id)
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        var prostori = await Context.Prostori.Where(x => x.VlasnikProstora == korisnik).ToListAsync();

        if (korisnik!.VlasnikProstori?.Find(x => x.ID == id) == null)
        {
            return BadRequest("You are not the owner of this space.");
        }

        var prostor = await Context.Prostori
                                    .Include(x => x.PlanoviProstora!)
                                    .ThenInclude(x => x.DraggableItems)
                                    .Include(x => x.PlanoviProstora!)
                                    .ThenInclude(x => x.Lines)
                                    .Include(x => x.PlanoviProstora!)
                                    .ThenInclude(x => x.SurfaceDimension)
                                    .FirstOrDefaultAsync(x => x.ID == id);

        if (prostor == null)
        {
            return BadRequest("Space not found.");
        }

        Context.Prostori.Remove(prostor);
        await Context.SaveChangesAsync();
        return Ok();
    }

}