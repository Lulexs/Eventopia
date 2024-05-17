using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class TokenService
{
    private readonly IConfiguration _config;
    private readonly UserManager<Korisnik> _userManager;
    public TokenService(IConfiguration config, UserManager<Korisnik> userManager)
    {
        _userManager = userManager;
        _config = config;
    }
    public async Task<string> CreateToken( Korisnik korisnik)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, korisnik.UserName),
            new Claim(ClaimTypes.NameIdentifier, korisnik.Id.ToString()),//ToString je trenutni fix samo da probam da pokrenem
            new Claim(ClaimTypes.Email, korisnik.Email),

        };

        var roles = await _userManager.GetRolesAsync(korisnik);//nalazim role za korisnika da ih stavim u token, mora da bude async jer pristupa bazi

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));//stavljam ih u token

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["TokenKey"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

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