using Backend.ApplicationLogic;
using Backend.ApplicationLogic.Exceptions;

namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
public class AdministratorController : ControllerBase
{
    public readonly AdministratorLogic _administratorLogic;
    private readonly UserManager<Korisnik> _userManager;
    private readonly Context _context;

    public AdministratorController(Context context, UserManager<Korisnik> userManager, AdministratorLogic administratorLogic)
    {
        _administratorLogic = administratorLogic;
        _userManager = userManager;
        _context = context;
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getUsersWithRoles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var korisnici = await _administratorLogic.GetUsersWithRoles();

        return Ok(korisnici);
    }

    // [Authorize(Policy = "RequireAdministratorRole")]
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

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getAllComments")]
    public async Task<ActionResult> GetAllComments()
    {
        var ocene = await _administratorLogic.GetAllComments();


        return Ok(ocene);
    }

    [HttpGet("test")]
    public async Task<IActionResult> Test()
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        DateTime now = DateTime.Now;
        int id1 = await _administratorLogic.BanUser(new BanUserDto
        {
            KorisnikId = "634a7ec8-1a5f-43a5-e98b-08dd25cb4a83",
            DatumOd = now,
            DatumDo = now.AddDays(7).ToString(),
            Razlog = "Test razlog"
        });

        int id2 = await _administratorLogic.BanUser(new BanUserDto
        {
            KorisnikId = "cb2f456a-0e50-43c9-e98a-08dd25cb4a83",
            DatumOd = now,
            DatumDo = now.AddDays(7).ToString(),
            Razlog = "Test razlog"
        });

        await transaction.RollbackAsync();

        return Ok();
    }

}