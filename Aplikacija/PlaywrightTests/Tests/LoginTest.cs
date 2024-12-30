using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.Tests;

[TestFixture]
public class LoginTest : PageTest
{

    private IBrowser _browser = null!;
    private IPage _page = null!;

    [SetUp]
    public async Task Setup()
    {
        _browser = await Playwright.Chromium.LaunchAsync(new()
        {
            Headless = false,
            SlowMo = 1000
        });
        _page = await _browser.NewPageAsync();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _page.CloseAsync();
        await _browser.CloseAsync();
    }

    [TestCase("eventvisitor1@gmail.com", "Sifra123!", "eventvisitor1@gmail.comEvent")]
    [TestCase("eventorganizer1@gmail.com", "Sifra123!", "eventorganizer1@gmail.comEvent")]
    [TestCase("spaceowner1@gmail.com", "Sifra123!", "spaceowner1@gmail.comSpace")]
    [TestCase("lulee@elfak.rs", "Sifra123!", "Luka")]
    [Ignore("Temp")]
    public async Task TestSuccessfullLogin(string email, string password, string expectedText)
    {
        var loginPage = new LoginPage(_page);

        await loginPage.GotoAsync("http://localhost:5173/login");
        await loginPage.LoginAsync(email, password);

        await Expect(_page.GetByRole(AriaRole.Banner)).ToContainTextAsync(expectedText);
    }

    [Test]
    [Ignore("Temp")]
    public async Task TestLoginWithWrongPassword()
    {
        var loginPage = new LoginPage(_page);

        await loginPage.GotoAsync("http://localhost:5173/login");

        string actualMessage = "";
        string expectedMessage = "Password is incorrect. Please try again.";

        var dialogHandled = new TaskCompletionSource<bool>();
        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            actualMessage = dialog.Message;
            dialog.DismissAsync();
            _page.Dialog -= Page_Dialog_EventHandler!;
            dialogHandled.TrySetResult(true);
        }

        _page.Dialog += Page_Dialog_EventHandler!;

        await loginPage.LoginAsync("eventvisitor1@gmail.com", "Sifra123");
        await dialogHandled.Task;

        Assert.That(actualMessage, Is.EqualTo(expectedMessage));
    }

    [Test]
    [Ignore("Temp")]
    public async Task TestLoginWithWrongEmailFormat()
    {
        var loginPage = new LoginPage(_page);

        await loginPage.GotoAsync("http://localhost:5173/login");
        await loginPage.LoginAsync("eventvisitor1", "Sifra123");

        await Expect(_page.GetByPlaceholder("example@gmail.com").Locator("xpath=../following-sibling::*")).ToContainTextAsync("Email required");
    }

}