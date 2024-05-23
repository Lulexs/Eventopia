using System.Security.Claims;
using API.DTOs;
using Backend.DTOs;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController : ControllerBase
{
    private readonly UserManager<Korisnik> _userManager;
    private readonly TokenService _tokenService;
    public AccountController(UserManager<Korisnik> userManager, TokenService tokenService)
    {
        _tokenService = tokenService;
        _userManager = userManager;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<KorisnikDto>> Login(LoginDto loginDto)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

        if (korisnik == null) return Unauthorized($"Netacan Vam je email : {loginDto.Email}");

        var result = await _userManager.CheckPasswordAsync(korisnik, loginDto.Password);//aspnet ovde proverava sifru da l se poklapa za nas

        if (result)
        {
            var korisnikObject = await CreateUserObject(korisnik);
            return korisnikObject;
        }

        return Unauthorized("Netacna Vam je sifra ");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<KorisnikDto>> Register(RegisterDto registerDto)
    {
        if (await _userManager.Users.AnyAsync(x => x.UserName == registerDto.Username))
        {
            ModelState.AddModelError("username", "Username taken");
            return ValidationProblem();
        }

        if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
        {
            ModelState.AddModelError("email", "Email taken");
            return ValidationProblem();
        }

        var korisnik = new Korisnik
        {
            Ime = registerDto.Ime,
            Prezime = registerDto.Prezime,
            Email = registerDto.Email,
            UserName = registerDto.Username,
            Telefon = registerDto.Telefon,
            DatumRodjenja = registerDto.DatumRodjenja,
            SlikaProfila = registerDto.Slika,
            // posle dodajem role za korisnika
        };


        var result = await _userManager.CreateAsync(korisnik, registerDto.Password);//da sacuvamo korisnika u bazu na osnovu passworda

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(korisnik, registerDto.UserRole);//implicitno mu za sad uvek dajemo Obicnog korisnika
            if (!roleResult.Succeeded) return BadRequest(roleResult.Errors);
            //ako prodje ovo gore onda kreiraj takvog korisnika
            var korisnikObject = await CreateUserObject(korisnik);
            return korisnikObject;
        }

        return BadRequest(result.Errors);
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<KorisnikDto>> GetCurrentUser()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        
        var korisnikObject = await CreateUserObject(korisnik);
        return korisnikObject;
    }

    private async Task<KorisnikDto> CreateUserObject(Korisnik korisnik)//promenjena u async metodu zbog novog nacina dobijanja tokena
    {
        return new KorisnikDto
        {
            Ime = korisnik.Ime,
            Prezime = korisnik.Prezime,
            Slika = korisnik?.SlikaProfila,
            Token = await _tokenService.CreateToken(korisnik),
            UserName = korisnik.UserName
        };
    }
}