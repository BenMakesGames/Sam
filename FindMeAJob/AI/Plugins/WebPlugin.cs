using System.ComponentModel;
using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using PuppeteerSharp;

namespace FindMeAJob.AI.Plugins;

public class WebPlugin
{
    private IBrowser? Browser { get; set; }
    private IPage? Page { get; set; }

    [KernelFunction]
    [Description("gets the contents of a page, given its URL")]
    public async Task<string> GetPageTextAndLinks(string url)
    {
        Console.WriteLine($"Sam > GetPageContents {url}");

        try
        {
            Browser ??= await Puppeteer.LaunchAsync(new LaunchOptions() { Headless = false }); // I wanna watch! :P
            Page ??= await Browser.NewPageAsync();

            await Page.GoToAsync(url, WaitUntilNavigation.Networkidle2);

            var document = new HtmlDocument();

            document.LoadHtml(await Page.GetContentAsync());

            var text = document.DocumentNode.InnerText;
            var links = document.DocumentNode.SelectNodes("//a[@href]").Select(x => x.Attributes["href"].Value).ToList();

            return $"TEXT {text}\n\nLINKS {string.Join(" ", links)}";
        }
        catch (Exception e)
        {
            return "EXCEPTION " + e.Message;
        }
    }
}
