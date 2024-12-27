
namespace UnitTests;

[SetUpFixture]
public class TestSetup
{

    [OneTimeSetUp]
    public async Task RunBeforeAnyTests()
    {
        var (_, _, _context) = UserManagerHelper.CreateUserManager();
        var migrator = _context.Database.GetService<IMigrator>();
        await migrator.MigrateAsync();

        string sqlFilePath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../data.sql"));
        var sqlCommands = await File.ReadAllTextAsync(sqlFilePath);
        await _context.Database.ExecuteSqlRawAsync(sqlCommands);

        _context.Dispose();
    }

    [OneTimeTearDown]
    public async Task RunAfterAllTests()
    {
        var (_, _, _context) = UserManagerHelper.CreateUserManager();
        await _context.Database.EnsureDeletedAsync();

        _context.Dispose();
    }
}