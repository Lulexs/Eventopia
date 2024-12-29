using Backend.ApplicationLogic;
using Backend.ApplicationLogic.Exceptions;

namespace Backend.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ReservationController : ControllerBase
{
    public Context Context { get; set; }
    private readonly UserManager<Korisnik> _userManager;
    private readonly ReservationLogic _reservationLogic;

    public ReservationController(ReservationLogic reservationLogic, Context context, UserManager<Korisnik> userManager)
    {
        _reservationLogic = reservationLogic;
        Context = context;
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpGet("getSpacePlan/{eventId}")]
    public async Task<ActionResult<SpaceDto?>> GetSpacePlan(int eventId)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        try
        {
            var spacePlan = await _reservationLogic.GetSpacePlan(eventId, korisnik!);
            return Ok(spacePlan);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("getEventDetails/{eventId}")]
    public async Task<ActionResult<EventDetailsDto?>> GetEventDetails(int eventId)
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        try
        {
            var eventDetails = await _reservationLogic.GetEventDetails(eventId, korisnik!);
            return Ok(eventDetails);
        }
        catch (UnauthorizedException e)
        {
            return Unauthorized(e.Message);
        }
    }

    [Authorize(Policy = "RequireVisitorRole")]
    [HttpPost("makeReservation/{tableId}/{numberOfSeats}")]
    public async Task<ActionResult> MakeReservation(int tableId, int numberOfSeats)
    {

        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));

        try
        {
            await _reservationLogic.MakeReservation(tableId, numberOfSeats, korisnik!);
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

}