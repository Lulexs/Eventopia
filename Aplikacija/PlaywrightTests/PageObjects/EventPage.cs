using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class EventPage {
    private readonly IPage _page;

    public EventPage(IPage page) {
        _page = page;
    }

}