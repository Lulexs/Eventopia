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

    [TestCase(null, "Sarajevo, Bosnia", null, new object[] { "pop" })]
    // [Ignore("Temp")]
    public async Task TestFilterEvents(string? eventName, string? location, string? date, object[]? tags)
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");

        if (tags != null)
        {
            await homePage.FilterEventsAsync(eventName, location, date, null);
        }
        else
        {
            await homePage.FilterEventsAsync(eventName, location, date, [.. (tags as string[])!]);
        }

    }
}