// using Microsoft.Playwright.NUnit;
// using Microsoft.Playwright;

// namespace PlaywrightTests;


// [Parallelizable(ParallelScope.Self)]
// [TestFixture]
// public class VisitorTests : PageTest
// {
//     IPage page;
//     IBrowser browser;

//     [SetUp]
//     public async Task Setup()
//     {
//         browser = await Playwright.Chromium.LaunchAsync(new()
//         {
//             Headless = false,
//             SlowMo = 2000
//         });
//         page = await browser.NewPageAsync(new()
//         {
//             ViewportSize = new()
//             {
//                 Width = 1280,
//                 Height = 720
//             },
//             ScreenSize = new()
//             {
//                 Width = 1280,
//                 Height = 720
//             }
//         });
//     }

//     [Test]
//     public async Task MyTest()
//     {
//         await page.GotoAsync("http://localhost:5173/login");
//         await page.GetByPlaceholder("example@gmail.com").ClickAsync();
//         await page.GetByPlaceholder("example@gmail.com").FillAsync("eventvisitor1@gmail.com");
//         await page.GetByPlaceholder("example@gmail.com").PressAsync("Tab");
//         await page.GetByPlaceholder("Your password").FillAsync("Sifra123!");
//         await Page.Locator("form").GetByRole(AriaRole.Button, new() { Name = "Log in" }).ClickAsync();
//         await Page.GetByText("eventvisitor1@gmail.comEvent").ClickAsync();
//         await Expect(page.GetByRole(AriaRole.Banner)).ToContainTextAsync("eventvisitor1@gmail.comEvent");
//     }
// }