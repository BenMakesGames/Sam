using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PuppeteerSharp;

namespace FindMeAJob.AI.Plugins;

public class WebPageLinksPlugin
{
    [KernelFunction]
    [Description("gets all links from a given page")]
    public async Task<string> GetWebPageLinks(Kernel kernel, string url)
    {
        Console.WriteLine($"Sam > GetWebPageLinks {url}");

        try
        {
            var page = await HorriblePuppeteerHelper.GoToAsync(url, WaitUntilNavigation.Networkidle2);

            return ExtractLinks(await page.GetContentAsync());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return "ERROR: " + e.Message;
        }
    }

    private static string ExtractLinks(string html)
    {
        var document = new HtmlDocument();

        document.LoadHtml(html);

        var links = document.DocumentNode.SelectNodes("//a[@href]")
            .Where(a => !string.IsNullOrWhiteSpace(a.GetAttributeValue("href", "")))
            .Select(a => $"{a.InnerText} - {a.GetAttributeValue("href", "")}")
            .ToList();

        var linkText = links.Count == 0
            ? "(no links found)"
            : string.Join("\n", links);

        Console.WriteLine($"Link Finder > {linkText}");

        return linkText;
    }
}
