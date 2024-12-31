using Microsoft.Playwright;
using NUnit.Framework.Internal.Execution;

namespace PlaywrightTests.PageObjects;

public class HomePage
{
    private readonly IPage _page;
    private string _url;

    public HomePage(IPage page)
    {
        _page = page;
        _url = "";
    }

    public async Task GotoAsync(string url)
    {
        _url = url;
        await _page.GotoAsync(url);
    }

    public async Task LoginAsync(string username, string password)
    {
        var loginPage = new LoginPage(_page);
        await loginPage.GotoAsync(_url + "/login");
        await loginPage.LoginAsync(username, password);
    }

    public async Task FilterEventsAsync(string? eventName, string? location, string? date, List<string>? tags)
    {
        if (eventName != null)
        {
            await _page.GetByPlaceholder("Enter event name...").FillAsync(eventName);
        }
        if (location != null)
        {
            await _page.GetByPlaceholder("Select location...").FillAsync(location);
        }
        if (date != null)
        {
            await _page.GetByPlaceholder("Pick a date...").FillAsync(date);
        }
        if (tags != null)
        {
            await _page.GetByPlaceholder("Select tags...").ClickAsync();
            foreach (var tag in tags)
            {
                await _page.GetByText(tag).ClickAsync();
            }
            await _page.GetByPlaceholder("Select tags...").ClickAsync();
        }
        await _page.GetByRole(AriaRole.Button, new() { Name = "Search" }).ClickAsync();
    }

    public async Task<EventPage> GotoEventPageAsync(string eventName)
    {
        await _page.GetByText("Explore events", new() { Exact = true }).ClickAsync();
        await _page.GetByText(eventName).Locator("xpath=../../button").ClickAsync();

        return new EventPage(_page);
    }

    public async Task<VisitorPage> GotoVisitorPage(string email)
    {
        await _page.GetByText(email).ClickAsync();

        return new VisitorPage(_page);
    }

    public async Task<EventHostPage> GotoHostPage(string email)
    {
        await _page.GetByText(email).ClickAsync();

        return new EventHostPage(_page);
    }
}