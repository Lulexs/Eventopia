using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class LoginPage
{
    private readonly IPage _page;

    private ILocator EmailInput => _page.GetByPlaceholder("example@gmail.com");
    private ILocator PasswordInput => _page.GetByPlaceholder("Your password");
    private ILocator LoginButton => _page.Locator("form").GetByRole(AriaRole.Button, new() { Name = "Log in" });

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoAsync(string url)
    {
        await _page.GotoAsync(url);
    }

    public async Task LoginAsync(string email, string password)
    {
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await LoginButton.ClickAsync();
    }
}