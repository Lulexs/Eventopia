using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.Tests;

[TestFixture]
public class VisitorTest : PageTest
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
    public async Task TestLeaveReview()
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");
        var visitorPage = await homePage.GotoVisitorPage("eventvisitor1@gmail.com");

        string actualMessage = "";
        string expectedMessage = "Review posted successfully!";

        var dialogHandled = new TaskCompletionSource<bool>();
        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            actualMessage = dialog.Message;
            dialog.DismissAsync();
            _page.Dialog -= Page_Dialog_EventHandler!;
            dialogHandled.TrySetResult(true);
        }

        _page.Dialog += Page_Dialog_EventHandler!;

        await visitorPage.LeaveAComment("Superisic", "This comment was written using pw");
        await dialogHandled.Task;
        Assert.That(actualMessage, Is.EqualTo(expectedMessage));

        await _page.ReloadAsync();
        await Expect(_page.GetByText(new Regex($"Superisic")).Locator("xpath=../following-sibling::button")).ToBeDisabledAsync(new LocatorAssertionsToBeDisabledOptions { Timeout = 60000 });

        await visitorPage.Logout();
        await homePage.LoginAsync("eventorganizer1@gmail.com", "Sifra123!");
        var eventHostPage = await homePage.GotoHostPage("eventorganizer1@gmail.com");
        await eventHostPage.GotoReviews("Superisic");
        await Expect(_page.GetByText("This comment was written using pw")).ToBeVisibleAsync();

    }

    [Test]
    // [Ignore("Temp")]
    public async Task TestCancelEvent()
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");
        var visitorPage = await homePage.GotoVisitorPage("eventvisitor1@gmail.com");

        await visitorPage.CancelReservation(5);
        await Expect(_page.GetByText(new Regex($"ID: {5}")).Locator("xpath=./preceding-sibling::*")).ToBeHiddenAsync();

        await homePage.GotoEventPageAsync("Jelena Tomasevic");
        var tables = await _page.QuerySelectorAllAsync("//img[@src='/src/assets/table_mine.png']");

        Assert.That(tables, Has.Count.EqualTo(1));
    }

    [Test]
    // [Ignore("Temp")]
    public async Task TestChangePersonalInfo()
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");
        var visitorPage = await homePage.GotoVisitorPage("eventvisitor1@gmail.com");

        string actualMessage = "";
        string expectedMessage = "Incorrect password.";

        var dialogHandled = new TaskCompletionSource<bool>();
        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            actualMessage = dialog.Message;
            dialog.DismissAsync();
            _page.Dialog -= Page_Dialog_EventHandler!;
            dialogHandled.TrySetResult(true);
        }

        _page.Dialog += Page_Dialog_EventHandler!;
        await visitorPage.ChangePersonalInformation(null, null, null, "068124245", "Sifra123!", "Sifra123");
        await dialogHandled.Task;
        Assert.That(actualMessage, Is.EqualTo(expectedMessage));

        _page.Dialog += Page_Dialog_EventHandler!;
        expectedMessage = "Successfully changed user info!";
        await visitorPage.ChangePersonalInformation(null, null, null, "068124245", "Sifra123!", "Sifra123!");
        await dialogHandled.Task;
        Assert.That(actualMessage, Is.EqualTo(expectedMessage));
    }

    [Test]
    // [Ignore("Temp")]
    public async Task TestChangeUserAvatarNTags()
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");
        var visitorPage = await homePage.GotoVisitorPage("eventvisitor1@gmail.com");


        string actualMessage = "";
        string expectedMessage = "User information is successfully changed!";

        var dialogHandled = new TaskCompletionSource<bool>();
        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            actualMessage = dialog.Message;
            dialog.DismissAsync();
            _page.Dialog -= Page_Dialog_EventHandler!;
            dialogHandled.TrySetResult(true);
        }

        _page.Dialog += Page_Dialog_EventHandler!;
        await visitorPage.ChangeAvatarNTags("5", ["rap", "hiphop"], ["jazz", "classical"]);

        await dialogHandled.Task;
        Assert.That(actualMessage, Is.EqualTo(expectedMessage));

        await Expect(visitorPage.AvatarInput).ToHaveAttributeAsync("src", new Regex("5"));

        var texts = await visitorPage.TagInput.Locator("xpath=./preceding-sibling::span/span").AllInnerTextsAsync();
        Assert.That(texts, Is.EquivalentTo(new List<string> { "jazz", "classical", "anything" }));
    }

    [Test]
    // [Ignore("Temp")]
    public async Task TestMakeReservationForEvent()
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");
        var eventPage = await homePage.GotoEventPageAsync("Lords of the Sound");

        // Table is alredy reserved
        await eventPage.ReserveSeat(316);
        await Expect(_page.GetByText("Table is already reserved!")).ToBeVisibleAsync();
        await _page.Locator("div")
                   .Filter(new() { HasTextRegex = new Regex("^Information about this table$") })
                   .GetByRole(AriaRole.Button).ClickAsync();

        // Reserving own table
        await eventPage.ReserveSeat(315);
        await Expect(_page.GetByText("You have reserved this table.")).ToBeVisibleAsync();
        await _page.Locator("div")
                   .Filter(new() { HasTextRegex = new Regex("^Information about this table$") })
                   .GetByRole(AriaRole.Button).ClickAsync();

        // Valid reservation
        string actualMessage = "";
        string expectedMessage = "Reservation made succesfully!";

        var dialogHandled = new TaskCompletionSource<bool>();
        void Page_Dialog_EventHandler(object sender, IDialog dialog)
        {
            actualMessage = dialog.Message;
            dialog.DismissAsync();
            _page.Dialog -= Page_Dialog_EventHandler!;
            dialogHandled.TrySetResult(true);
        }

        _page.Dialog += Page_Dialog_EventHandler!;
        await eventPage.ReserveSeat(314, 4);

        await dialogHandled.Task;
        Assert.That(actualMessage, Is.EqualTo(expectedMessage));
        await Expect(eventPage.SeatLocator(314)).ToHaveAttributeAsync("src", new Regex("/src/assets/table_mine.png"));
    }

    [TestCase(null, "Sarajevo, Bosnia", null, new object[] { "pop" }, new object[] { "Jelena Tomasevic" })]
    [TestCase("Heavy Lungs", null, null, null, new object[] { "Heavy Lungs" })]
    [TestCase(null, "Nis, Serbia", null, null, new object[] { "Heavy Lungs", "Lords of the Sound" })]
    [TestCase(null, null, "", null, new object[] { "Bojan Sudjic", "Lords of the Sound" })]
    // [Ignore("Temp")]
    public async Task TestFilterEvents(string? eventName, string? location, string? date, object[]? tags, object[] expectedEvents)
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");

        if (date != null)
        {
            DateTime now = DateTime.Now;
            date = now.AddDays(4).ToString("MM/dd/yyyy");
        }

        if (tags == null)
        {
            await homePage.FilterEventsAsync(eventName, location, date, null);
        }
        else
        {
            List<string> tagsList = [];
            for (int i = 0; i < tags.Length; ++i)
            {
                tagsList.Add(tags[i].ToString()!);
            }
            await homePage.FilterEventsAsync(eventName, location, date, tagsList);
        }

        var cards = await _page.QuerySelectorAllAsync(
            $"xpath=//h1[normalize-space(text())='Explore, Connect, Experience:']/following-sibling::*[2]//div[2]/p"
        );

        var names = await Task.WhenAll(cards.Select(x => x.InnerTextAsync()));

        Assert.That(names, Is.EquivalentTo(expectedEvents));
    }
}