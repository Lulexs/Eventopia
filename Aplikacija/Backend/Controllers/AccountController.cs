namespace Backend.Controllers;

[ApiController]
[Route("[controller]")]
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
    public async Task<ActionResult<KorisnikDto>> Login([FromBody] LoginDto loginDto)
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == loginDto.Email);

        if (korisnik == null) return Unauthorized($"User with this email doesn't exist: {loginDto.Email}");

        var result = await _userManager.CheckPasswordAsync(korisnik, loginDto.Password);

        if (result)
        {
            var korisnikObject = await CreateUserObject(korisnik);
            return korisnikObject;
        }

        return Unauthorized("Password is incorrect. Please try again.");
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<KorisnikDto>> Register([FromBody] RegisterDto registerDto)
    {

        if (await _userManager.Users.AnyAsync(x => x.Email == registerDto.Email))
        {
            return ValidationProblem("Email is already in use.");
        }

        if (registerDto.UserType == "Admin")
        {
            registerDto.UserType = "Visitor";
        }

        var korisnik = new Korisnik
        {
            Ime = registerDto.Ime,
            Prezime = registerDto.Prezime,
            Email = registerDto.Email,
            UserName = registerDto.Email,
            Telefon = registerDto.Telefon,
            DatumRodjenja = registerDto.DatumRodjenja,
            SlikaProfila = registerDto.Slika,
            Adresa = registerDto.Adresa,
            Grad = registerDto.Grad
        };

        var result = await _userManager.CreateAsync(korisnik, registerDto.Password);

        if (result.Succeeded)
        {
            var roleResult = await _userManager.AddToRoleAsync(korisnik, registerDto.UserType);
            if (!roleResult.Succeeded)
                return BadRequest(roleResult.Errors);
            var korisnikObject = await CreateUserObject(korisnik);
            return korisnikObject;
        }

        return BadRequest(result.Errors);
    }

    [Authorize]
    [HttpGet("getCurrentUser")]
    public async Task<ActionResult<KorisnikDto>> GetCurrentUser()
    {
        var korisnik = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == User.FindFirstValue(ClaimTypes.Email));
        var korisnikObject = await CreateUserObject(korisnik!);
        return korisnikObject;
    }

    private async Task<KorisnikDto> CreateUserObject(Korisnik korisnik)
    {
        return new KorisnikDto
        {
            FirstName = korisnik.Ime,
            LastName = korisnik.Prezime,
            Token = await _tokenService.CreateToken(korisnik),
            DateOfBirth = korisnik.DatumRodjenja,
            PhoneNumber = korisnik.Telefon,
            Avatar = korisnik.SlikaProfila,
            Address = korisnik.Adresa,
            City = korisnik.Grad
        };
    }
}