using Backend.ApplicationLogic;
using Backend.ApplicationLogic.Exceptions;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AdministratorController : ControllerBase
{
    public readonly AdministratorLogic _administratorLogic;
    private readonly UserManager<Korisnik> _userManager;

    public AdministratorController(UserManager<Korisnik> userManager, AdministratorLogic administratorLogic)
    {
        _administratorLogic = administratorLogic;
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getUsersWithBans")]
    public async Task<ActionResult<List<KorisnikSaZabranamaDto>>> GetUsersWithBans()
    {
        var korisniciSaZabranama = await _administratorLogic.GetUsersWithBans();

        return Ok(korisniciSaZabranama);

    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpPost("banUser")]
    public async Task<ActionResult> BanUser([FromBody] BanUserDto banUserDto)
    {
        try
        {
            var admin = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
            await _administratorLogic.BanUser(banUserDto);
        }
        catch (AlreadyBannedException e)
        {
            return BadRequest(e.Message);
        }
        catch (UserNotFoundException e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpDelete("unbanUser/{zabranaId}")]
    public async Task<ActionResult> UnbanUser(int zabranaId)
    {
        try
        {
            await _administratorLogic.UnbanUser(zabranaId);
        }
        catch (BanNotFoundException e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpDelete("deleteEvent/{id}")]
    public async Task<ActionResult> DeleteEvent(int id)
    {
        try
        {
            await _administratorLogic.DeleteEvent(id);
        }
        catch (EventNotFoundException e)
        {
            return BadRequest(e.Message);
        }

        return Ok();
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getAllEvents")]
    public async Task<ActionResult<List<DogadjajDto>>> GetAllEvents()
    {
        var dogadjaji = await _administratorLogic.GetAllEvents();

        return Ok(dogadjaji);
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpDelete("deleteComment/{id}")]
    public async Task<ActionResult> DeleteComment(int id)
    {
        await _administratorLogic.DeleteComment(id);

        return Ok();
    }

    // [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getAllComments")]
    public async Task<ActionResult> GetAllComments()
    {
        var ocene = await _administratorLogic.GetAllComments();

        return Ok(ocene.Select(x => new { x.Id, Comment = x.Komentar }));
    }
}