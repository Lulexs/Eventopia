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

    [TestCase(null, "Sarajevo, Bosnia", null, new object[] { "pop" }, new object[] { "Jelena Tomasevic" })]
    // [Ignore("Temp")]
    public async Task TestFilterEvents(string? eventName, string? location, string? date, object[]? tags, object[] expectedEvents)
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("eventvisitor1@gmail.com", "Sifra123!");

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