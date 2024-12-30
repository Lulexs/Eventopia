using Microsoft.Playwright;

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
}