using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class SpaceOwnerPage
{
    private IPage _page;

    public SpaceOwnerPage(IPage page)
    {
        _page = page;
    }

    public async Task NewSpaceAsync()
    {
        await _page.GetByRole(AriaRole.Button, new() { Name = "New space " }).ClickAsync();

        await _page.GetByLabel("City *").FillAsync("Nis");
        await _page.GetByLabel("Country *").FillAsync("Serbia");
        await _page.GetByLabel("Address *", new() { Exact = true }).FillAsync("Test adresa 15");

        await _page.GetByText("Corner").ClickAsync();
        await _page.Mouse.ClickAsync(200, 250);
        await _page.Mouse.ClickAsync(800, 250);
        await _page.Mouse.ClickAsync(200, 700);
        await _page.Mouse.ClickAsync(800, 700);

        await _page.Mouse.ClickAsync(210, 260);
        await _page.Mouse.ClickAsync(810, 260);

        await _page.Mouse.ClickAsync(810, 710);
        await _page.Mouse.ClickAsync(810, 260);

        await _page.Mouse.ClickAsync(210, 710);
        await _page.Mouse.ClickAsync(810, 710);

        await _page.Mouse.ClickAsync(210, 260);
        await _page.Mouse.ClickAsync(210, 710);

        await _page.GetByText("Table").ClickAsync();
        for (int i = 0; i < 10; ++i)
        {
            for (int j = 0; j < 7; ++j)
            {
                await _page.Mouse.ClickAsync(250 + 50 * i, 260 + j * 50);
            }
        }

        await _page.GetByRole(AriaRole.Button, new() { Name = "Submit" }).ClickAsync();
    }
}