using Backend.ApplicationLogic;
using Backend.ApplicationLogic.Exceptions;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class HostController : ControllerBase
{
    private readonly UserManager<Korisnik> _userManager;
    public Context Context { get; set; }
    private readonly HostLogic _hostLogic;
    public HostController(HostLogic hostLogic, Context context, UserManager<Korisnik> userManager)
    {
        Context = context;
        _userManager = userManager;
        _hostLogic = hostLogic;
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

        try
        {
            var dogadjaj = await _hostLogic.CreateEvent(createEventDto, korisnik);
            return Ok(dogadjaj);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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

        try
        {
            var spaces = _hostLogic.GetAvailableSpaces(date, time, location, capacity, korisnik);
            return Ok(spaces);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
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

        try
        {
            var spacePlan = await _hostLogic.GetSpacePlan(spaceId, korisnik);
            return Ok(spacePlan);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpDelete("cancelEvent/{id}")]
    public async Task<IActionResult> CancelEvent([FromRoute] int id)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            await _hostLogic.CancelEvent(id, korisnik);
            return Ok();
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getIncomingEvents")]
    public async Task<ActionResult<List<EventForListDto>>> GetIncomingEvents()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            var dogadjaji = await _hostLogic.GetIncomingEvents(korisnik);
            return Ok(dogadjaji);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
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

        try
        {
            var dogadjaji = await _hostLogic.GetPastEvents(korisnik);
            return Ok(dogadjaji);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getStatistics")]
    public async Task<ActionResult> GetStatistics()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            var statistics = _hostLogic.GetStatistics(korisnik);
            return Ok(statistics);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getReviewsForEvent/{id}")]
    public async Task<ActionResult> GetReviewsForEvent(int id)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            var ocene = _hostLogic.GetReviewsForEvent(id, korisnik);
            return Ok(ocene);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }


    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getEventDetails/{id}")]
    public async Task<ActionResult> GetEventDetails(int id)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            var dogadjaj = _hostLogic.GetEventDetails(id, korisnik);
            return Ok(dogadjaj);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpPut("changeEventDetails")]
    public async Task<ActionResult> ChangeEventDetails([FromBody] ChangeEventDto changeEventDto)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            await _hostLogic.ChangeEventDetails(changeEventDto, korisnik);
            return Ok();
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getReservations/{id}")]
    public async Task<ActionResult> GetReservations(int id)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            var res = _hostLogic.GetReservations(id, korisnik);
            return Ok(res);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }

    }

    [Authorize(Policy = "RequireHostRole")]
    [HttpGet("getEventSpace/{eventId}")]
    public async Task<ActionResult> GetEventSpace(int eventId)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        if (korisnik == null)
        {
            return NotFound("User not found.");
        }

        try
        {
            var spacePlan = _hostLogic.GetEventSpace(eventId, korisnik);
            return Ok(spacePlan);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

}
