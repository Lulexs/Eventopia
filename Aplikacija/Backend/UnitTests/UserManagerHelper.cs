using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests;

public static class UserManagerHelper
{
    public static (UserManager<Korisnik> userManager, RoleManager<AppRole> roleManager, Context context) CreateUserManager()
    {
        var services = new ServiceCollection();

        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionString = configuration.GetConnectionString("TestCS");

        var options = new DbContextOptionsBuilder<Context>()
            .UseSqlServer(connectionString, options => options.MigrationsAssembly("Backend"))
            .Options;

        var context = new Context(options);
        services.AddSingleton(context);

        services.AddIdentityCore<Korisnik>(opt =>
        {
            opt.Password.RequireNonAlphanumeric = true;
            opt.User.RequireUniqueEmail = true;
        })
        .AddRoles<AppRole>()
        .AddRoleManager<RoleManager<AppRole>>()
        .AddEntityFrameworkStores<Context>();

        var serviceProvider = services.BuildServiceProvider();

        var userManager = serviceProvider.GetRequiredService<UserManager<Korisnik>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();

        return (userManager, roleManager, context);
    }
}