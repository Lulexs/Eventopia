using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace Backend.Controllers;

public class AdministratorController : ControllerBase
{
    private readonly UserManager<Korisnik> _userManager;
    public AdministratorController(UserManager<Korisnik> userManager)
        {
            _userManager = userManager;
        }


    [Authorize(Policy = "RequireAdministratorRole")]
        [HttpGet("VratiSveKorisnikeSaUlogama")]
        public async Task<IActionResult> VratiSveKorisnikeSaUlogama()
        {
            var korisnici = await _userManager.Users.OrderBy(korisnik => korisnik.UserName).Select(korisnik => new  {
                    korisnik.Id,
                    Username = korisnik.UserName,
                    Role = korisnik.UserRole
            }).ToListAsync();
            return Ok(korisnici);
        }
}