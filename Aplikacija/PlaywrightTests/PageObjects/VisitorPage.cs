using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class VisitorPage
{
    private readonly IPage _page;

    public VisitorPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoAsync()
    {
        await _page.GotoAsync("http://localhost:5173/login");
        await _page.GetByPlaceholder("example@gmail.com").FillAsync("eventvisitor1@gmail.com");
        await _page.GetByPlaceholder("Your password").FillAsync("Sifra123!");
        await _page.Locator("form").GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
        await _page.GetByText("eventvisitor1@gmail.comEvent").ClickAsync();
    }
}