using System.Text.RegularExpressions;
using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class EventHostPage
{
    private readonly IPage _page;

    public EventHostPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoReviews(string eventName)
    {
        await _page.GetByText(new Regex(eventName)).Locator("xpath=../following-sibling::button").ClickAsync(new LocatorClickOptions { Timeout = 60000 });
    }
}