using Microsoft.Playwright;

namespace PlaywrightTests.PageObjects;

public class EventPage
{
    private readonly IPage _page;

    public ILocator SeatLocator(int seatNum) => _page.GetByRole(AriaRole.Img, new() { Name = $"table-{seatNum}" });

    public EventPage(IPage page)
    {
        _page = page;
    }

    public async Task ReserveSeat(int tableId, int? numOfSeatsToReserve = null)
    {
        await SeatLocator(tableId).ClickAsync();

        if (numOfSeatsToReserve != null)
        {
            await _page.GetByPlaceholder("Number of seats...").FillAsync($"{numOfSeatsToReserve}");
            await _page.GetByRole(AriaRole.Button, new() { Name = "Make a reservation" }).ClickAsync();
        }
    }

}