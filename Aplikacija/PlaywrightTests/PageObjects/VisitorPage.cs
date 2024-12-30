using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class VisitorPage
{
    private readonly IPage _page;

    public ILocator EmailInput => _page.GetByLabel("Email");
    public ILocator FirstNameInput => _page.GetByLabel("First name");
    public ILocator LastNameInput => _page.GetByLabel("Last name");
    public ILocator BirthdayInput => _page.GetByLabel("Birthday");
    public ILocator PhoneInput => _page.GetByLabel("Phone number");
    public ILocator AvatarInput => _page.GetByRole(AriaRole.Img, new() { Name = "avatar currently unavailable" });

    public VisitorPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoAsync(string url)
    {
        await _page.GotoAsync(url);
    }
}