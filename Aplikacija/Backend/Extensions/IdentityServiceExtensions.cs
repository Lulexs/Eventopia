//definises
using System.Text;
using Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        //povezamo sa aspnet identity klasom kazemo da ne zahteva alfanumericke posebne sifra i da email mora da bude jedinstven
        services.AddIdentityCore<Korisnik>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = true;

        }).AddRoles<AppRole>()
          .AddRoleManager<RoleManager<AppRole>>()
          .AddEntityFrameworkStores<Context>(); //da kreira sve tabele povezane sa identitijem
          //ovo dole je da kreira sve tabele povezane sa identityem

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"])); //kljuc za kriptovanje tokena
    
        //da se proveri validacija tokena
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = key,
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                    opt.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];
                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) && (path.StartsWithSegments("/chat")))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });
        //pravim polise za autorizaciju koje ce da se koriste iznad funkcija da se odredi ko sme da pozove fju
        // bez toga bih radio [Authorize(Roles = "Administrator") ]
        //ali sa ovim mogu da radim :[Authorize(Policy = "RequireAdministratorRole")]
        services.AddAuthorization(option => {
            option.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
            option.AddPolicy("RequireVisitor", policy => policy.RequireRole("Visitor"));
            option.AddPolicy("RequireSpaceOwner", policy => policy.RequireRole("Space owner"));
            option.AddPolicy("RequireHost", policy => policy.RequireRole("Host"));
        });

        services.AddScoped<TokenService>();
        return services;
    }
}