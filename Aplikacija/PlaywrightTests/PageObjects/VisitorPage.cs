using System.Text.RegularExpressions;
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
    public ILocator PasswordInput => _page.GetByPlaceholder("New password");
    public ILocator CurrentPasswordInput => _page.GetByPlaceholder("Enter current password");
    public ILocator AvatarInput => _page.GetByRole(AriaRole.Img, new() { Name = "avatar currently unavailable" });
    public ILocator SavePersonalInfoChangesButton => _page.GetByRole(AriaRole.Group, new() { Name = "Personal information" }).GetByRole(AriaRole.Button);
    public ILocator TagInput => _page.GetByPlaceholder("Enter tag");
    public ILocator SaveAvatarNTagInfoButton => _page.GetByRole(AriaRole.Group, new() { Name = "Avatar & Tags" }).GetByRole(AriaRole.Button);

    public VisitorPage(IPage page)
    {
        _page = page;
    }

    public async Task GotoAsync(string url)
    {
        await _page.GotoAsync(url);
    }

    public async Task ChangePersonalInformation(string? firstName, string? lastName, string? birthDay, string? phoneNumber, string? newPassword, string? oldPassword)
    {
        if (firstName != null)
            await FirstNameInput.FillAsync(firstName);
        if (lastName != null)
            await LastNameInput.FillAsync(lastName);
        if (birthDay != null)
        {
            await BirthdayInput.FillAsync(birthDay);
            await _page.GetByRole(AriaRole.Heading, new() { Name = "User info" }).ClickAsync();
        }
        if (phoneNumber != null)
            await PhoneInput.FillAsync(phoneNumber);
        if (newPassword != null && oldPassword != null)
        {
            await PasswordInput.FillAsync(newPassword);
            await CurrentPasswordInput.FillAsync(oldPassword);
        }
        await SavePersonalInfoChangesButton.ClickAsync();
    }

    public async Task ChangeAvatarNTags(string? avatarNum, List<string> tagsToRemove, List<string> tagsToAdd)
    {
        if (avatarNum != null)
        {
            await AvatarInput.ClickAsync();
            await _page.Locator($"img[src*='/avatar-{avatarNum}.png']").ClickAsync();
        }
        foreach (var tag in tagsToRemove)
        {
            await _page.GetByText(tag).Locator("xpath=../button").ClickAsync();
        }
        foreach (var tag in tagsToAdd)
        {
            await _page.GetByPlaceholder("Enter tag").FillAsync(tag);
            await _page.GetByPlaceholder("Enter tag").PressAsync("Enter");
        }
        await SaveAvatarNTagInfoButton.ClickAsync();
    }

    public async Task CancelReservation(int reservationId)
    {
        await _page.GetByText(new Regex($"ID: {reservationId}")).Locator("xpath=../preceding-sibling::button").ClickAsync();
    }

    public async Task GotoHomePageAsync()
    {
        await _page.GetByRole(AriaRole.Link, new() { Name = "Home" }).ClickAsync();
    }

    public async Task LeaveAComment(string eventName, string comment)
    {
        await _page.ReloadAsync();
        await _page.GetByText(new Regex($"{eventName}")).Locator("xpath=../following-sibling::button").ClickAsync(new LocatorClickOptions { Timeout = 60000 });

        await _page.GetByRole(AriaRole.Slider).HoverAsync();
        await _page.Mouse.DownAsync();
        await _page.Mouse.MoveAsync(50, 0);
        await _page.Mouse.UpAsync();
        await _page.GetByPlaceholder("Event was enjoyable...").FillAsync(comment);
        await _page.GetByRole(AriaRole.Button, new() { Name = "Post review" }).ClickAsync();
    }

    public async Task Logout()
    {
        await _page.GetByRole(AriaRole.Button, new() { Name = "Log out" }).ClickAsync();
    }
}