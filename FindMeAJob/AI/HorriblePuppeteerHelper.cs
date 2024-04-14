using PuppeteerSharp;

namespace FindMeAJob.AI;

public class HorriblePuppeteerHelper
{
    private static IBrowser? Browser { get; set; }
    private static IPage? Page { get; set; }

    public static async Task<IPage> GoToAsync(string url, WaitUntilNavigation waitUntil)
    {
        await new BrowserFetcher().DownloadAsync();

        Browser ??= await Puppeteer.LaunchAsync(new LaunchOptions() { Headless = false }); // I wanna watch! :P
        Page ??= await Browser.NewPageAsync();

        await Page.GoToAsync(url, waitUntil);

        return Page;
    }
}
