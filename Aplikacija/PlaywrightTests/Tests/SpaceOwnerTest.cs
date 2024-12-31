using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using PlaywrightTests.PageObjects;

namespace PlaywrightTests.Tests;

[TestFixture]
public class SpaceOwnerTest : PageTest
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
    [Ignore("Temp")]
    public async Task TestAddingAndRemovingNewSpace()
    {
        var homePage = new HomePage(_page);
        await homePage.GotoAsync("http://localhost:5173");
        await homePage.LoginAsync("spaceowner1@gmail.com", "Sifra123!");
        var spaceOwnerPage = await homePage.GotoSpaceOwnerPage("spaceowner1@gmail.com");

        await spaceOwnerPage.NewSpaceAsync();

        await Expect(_page.GetByText("Rentable spaces").Locator("xpath=./following-sibling::p")).ToHaveTextAsync("3");
        await Expect(_page.GetByText(new Regex("Test adresa 15Capacity:"))).ToBeVisibleAsync();
        await Expect(_page.GetByText(new Regex("Test adresa 15Capacity:"))).ToContainTextAsync("280");
    }
}