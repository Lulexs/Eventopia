namespace Backend.Extensions;

public static class IdentityServiceExtensions
{
    public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {

        services.AddIdentityCore<Korisnik>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = false;
            opt.User.RequireUniqueEmail = true;

        }).AddRoles<AppRole>()
          .AddRoleManager<RoleManager<AppRole>>()
          .AddEntityFrameworkStores<Context>();

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtSettings:TokenKey"]!));

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
                });

        services.AddAuthorization(option =>
        {
            option.AddPolicy("RequireAdministratorRole", policy => policy.RequireRole("Admin"));
            option.AddPolicy("RequireVisitor", policy => policy.RequireRole("Visitor"));
            option.AddPolicy("RequireSpaceOwner", policy => policy.RequireRole("Space owner"));
            option.AddPolicy("RequireHost", policy => policy.RequireRole("Host"));
        });

        services.AddScoped<TokenService>();
        return services;
    }
}