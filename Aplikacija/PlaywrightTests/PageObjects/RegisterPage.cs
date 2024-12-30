using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class RegisterPage
{
    private readonly IPage _page;

    private ILocator FirstNameLocator => _page.GetByPlaceholder("John");
    private ILocator LastNameLocator => _page.GetByPlaceholder("Doe");
    private ILocator EmailInput => _page.GetByPlaceholder("example@gmail.com");
    private ILocator PasswordInput => _page.GetByPlaceholder("Your password");
    private ILocator RepeatPassword => _page.GetByPlaceholder("Selected password");
    private ILocator PhoneNumberInput => _page.GetByPlaceholder("012456789");
    private ILocator BirthdayInput => _page.GetByLabel("Birthday *");
    private ILocator AvatarInput(int imgNum)
    {
        if (imgNum == 1)
        {
            return _page.Locator("img[src*='/avatar-1.png']");
        }
        else
        {
            return _page.Locator($"img:nth-child({imgNum})");
        }
    }
    private ILocator RegisterButton => _page.Locator("form").GetByRole(AriaRole.Button, new() { Name = "Sign up" });

    public RegisterPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoAsync(string url)
    {
        await _page.GotoAsync(url);
    }

    public async Task RegisterAsync(string firstName, string lastName, string email, string password, string repeatPassword, string phoneNumber, string birthday, int avatarImg)
    {
        await FirstNameLocator.FillAsync(firstName);
        await LastNameLocator.FillAsync(lastName);
        await EmailInput.FillAsync(email);
        await PasswordInput.FillAsync(password);
        await RepeatPassword.FillAsync(repeatPassword);
        await PhoneNumberInput.FillAsync(phoneNumber);
        await BirthdayInput.FillAsync(birthday);
        await FirstNameLocator.ClickAsync();
        await AvatarInput(avatarImg).ClickAsync();
        await RegisterButton.ClickAsync();
    }
}