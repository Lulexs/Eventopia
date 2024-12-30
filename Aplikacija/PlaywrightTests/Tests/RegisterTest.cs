using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.Tests;

[TestFixture]
public class RegisterTest : PageTest
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

    [Test]
    // [Ignore("Temp")]
    public async Task RegisterUserAlreadyExists()
    {
        var registerPage = new RegisterPage(_page);
        await registerPage.GotoAsync("http://localhost:5173/register");
        var dialogHandled = new TaskCompletionSource<bool>();

        string actualMessage = "";
        string expectedMessage = "Email is already in use.";

        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            actualMessage = dialog.Message;
            dialog.DismissAsync();
            _page.Dialog -= Page_Dialog_EventHandler!;
            dialogHandled.TrySetResult(true);
        }

        _page.Dialog += Page_Dialog_EventHandler!;

        await registerPage.RegisterAsync("Event", "Visitor1", "eventvisitor1@gmail.com", "Sifra123!", "Sifra123!", "062/2222122", "5/5/1991", 5);
        await dialogHandled.Task;

        Assert.That(actualMessage, Is.EqualTo(expectedMessage));
    }
}