// servis u kome definisiemo sta sve se prenosi kroz token kako bi se korisnik 
// autentifikovao i autorizovao i to se radi npr:
//[Authorize(Policy = "RequireAdministratorRole")]
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<Korisnik> _userManager;
    public TokenService(IConfiguration config, UserManager<Korisnik> userManager)
    {
        _userManager = userManager;
        _config = config;
    }
    public async Task<string> CreateToken(Korisnik korisnik)
    {

        //claims je sta ce token da zna o korisniku, to su username, id, email irole ovo dole na kraju
        var claims = new List<Claim>();

        if (!string.IsNullOrEmpty(korisnik.UserName))
        {
            claims.Add(new Claim(ClaimTypes.Name, korisnik.UserName));
        }


        claims.Add(new Claim(ClaimTypes.NameIdentifier, korisnik.Id.ToString()));


        if (!string.IsNullOrEmpty(korisnik.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, korisnik.Email));
        }

        var roles = await _userManager.GetRolesAsync(korisnik);//nalazim role za korisnika da ih stavim u token, mora da bude async jer pristupa bazi

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));//stavljam ih u token, retardirano malo, ali tako je

        //kriptovanje tokena
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

        //opis tokena, kolko ce dugo da trajee, sta ce da ima u sebi i kako ce da se potpise
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}