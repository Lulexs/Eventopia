using Backend.ApplicationLogic;
using Backend.Models;
using Microsoft.AspNetCore.Identity;

namespace UnitTests;

[TestFixture]
public class AdministratorLogicTests
{
    private UserManager<Korisnik> _userManager = null!;
    private Context _context = null!;
    private AdministratorLogic _adminLogic = null!;

    [OneTimeSetUp]
    public void BeforeAllSetup()
    {
        (_userManager, _, _context) = UserManagerHelper.CreateUserManager();
        _adminLogic = new AdministratorLogic(_userManager, _context);
    }

    [OneTimeTearDown]
    public void AfterAllTests()
    {
        _context.Dispose();
    }

    [Test]
    public async Task GetBannedUsers_ReturnsEmptyList()
    {
        var bannedUsers = await _adminLogic.GetUsersWithBans();
        Assert.That(bannedUsers, Is.Empty);
    }
}