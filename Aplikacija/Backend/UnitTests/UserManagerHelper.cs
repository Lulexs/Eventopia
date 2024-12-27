using Backend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace UnitTests;

public static class UserManagerHelper
{
    public static (UserManager<Korisnik> userManager, RoleManager<AppRole> roleManager, Context context) CreateUserManager()
    {
        var services = new ServiceCollection();

        var options = new DbContextOptionsBuilder<Context>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
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