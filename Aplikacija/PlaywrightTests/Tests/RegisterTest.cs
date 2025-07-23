using System.Text.RegularExpressions;
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

    [TestCase("Test", "Visitor", "testvisitor@gmail.com", "Sifra123!", "Sifra123!", "062/2222212", "May 5, 1991", 5)]
    // [Ignore("Temp")]
    public async Task RegisterVisitor(string firstName, string lastName,
                                      string email, string password, string repeatPassword,
                                      string phoneNumber, string birthDay, int avatarNum)
    {
        var registerPage = new RegisterPage(_page);
        await registerPage.GotoAsync("http://localhost:5173/register");
        await registerPage.RegisterAsync(firstName, lastName, email, password, repeatPassword, phoneNumber, birthDay, avatarNum);

        await _page.WaitForTimeoutAsync(5000);
        var visitorPage = new VisitorPage(_page);
        await visitorPage.GotoAsync("http://localhost:5173/visitorprofile");

        await Expect(visitorPage.EmailInput).ToHaveValueAsync(email);
        await Expect(visitorPage.FirstNameInput).ToHaveValueAsync(firstName);
        await Expect(visitorPage.LastNameInput).ToHaveValueAsync(lastName);
        await Expect(visitorPage.BirthdayInput).ToHaveValueAsync(birthDay);
        await Expect(visitorPage.PhoneInput).ToHaveValueAsync(phoneNumber);
        await Expect(visitorPage.AvatarInput).ToHaveAttributeAsync("src", new Regex("5"));
    }

    [Test]
    // [Ignore("Temp")]
    public async Task RegisterNoValues()
    {
        var registerPage = new RegisterPage(_page);
        await registerPage.GotoAsync("http://localhost:5173/register");
        await registerPage.RegisterAsync("", "", "", "", "", "", "", 1);

        await Expect(_page.GetByPlaceholder("John").Locator("xpath=../following-sibling::*")).ToContainTextAsync("First name required");
        await Expect(_page.GetByPlaceholder("Doe").Locator("xpath=../following-sibling::*")).ToContainTextAsync("Last name required");
        await Expect(_page.GetByPlaceholder("example@gmail.com").Locator("xpath=../following-sibling::*")).ToContainTextAsync("Email required");
        await Expect(_page.GetByPlaceholder("Your password").Locator("xpath=../../following-sibling::*")).ToContainTextAsync("Password required");
        await Expect(_page.GetByPlaceholder("012456789").Locator("xpath=../following-sibling::*")).ToContainTextAsync("Phone number required");
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