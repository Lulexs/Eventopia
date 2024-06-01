namespace Backend.Controllers;

public class AdministratorController : ControllerBase
{
    private readonly UserManager<Korisnik> _userManager;

    public AdministratorController(UserManager<Korisnik> userManager)
    {
        _userManager = userManager;
    }

    [Authorize(Policy = "RequireAdministratorRole")]
    [HttpGet("getUsersWithRoles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var korisnici = await _userManager.Users.OrderBy(korisnik => korisnik.Ime)
                                                .ThenBy(korisnik => korisnik.Prezime)
                                                .Select(korisnik => new
                                                {
                                                    korisnik.Id,
                                                    korisnik.Ime,
                                                    korisnik.Prezime,
                                                    korisnik.Email,
                                                    Role = korisnik.UserRole
                                                }).ToListAsync();
        return Ok(korisnici);
    }
}