using System.ComponentModel;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using PuppeteerSharp;

namespace FindMeAJob.AI.Plugins;

public class WebPageJobSummariesPlugin
{
    private IBrowser? Browser { get; set; }
    private IPage? Page { get; set; }

    [KernelFunction]
    [Description("gets a summary of a web page, including any job postings found on that web page")]
    public async Task<string> GetWebPageJobSummaries(Kernel kernel, string url)
    {
        Console.WriteLine($"Sam called GetWebPageJobSummaries {url}");

        try
        {
            var page = await HorriblePuppeteerHelper.GoToAsync(url, WaitUntilNavigation.Networkidle2);

            return await Summarize(kernel, await page.GetContentAsync())
               ?? "ERROR: Unable to produce a summary.";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return "ERROR: " + e.Message;
        }
    }

    private static async Task<string?> Summarize(Kernel kernel, string html)
    {
        // the raw HTML can be too long... I don't LOVE this way of reducing it (we lose a lot of context), but it's a start:
        var document = new HtmlDocument();

        document.LoadHtml(html);

        var text = document.DocumentNode.InnerText;
        text = Regex.Replace(text, @"\s+", " ");

        var links = document.DocumentNode.SelectNodes("//a[@href]")
            .Where(a => !string.IsNullOrWhiteSpace(a.GetAttributeValue("href", "")))
            .Select(a => $"{a.InnerText} - {a.GetAttributeValue("href", "")}")
            .ToList();

        var stupidText = $"TEXT: {text}\n\nLINKS: {string.Join("\n", links)}";

        var history = new ChatHistory();

        history.AddUserMessage("Please extract the details of any job postings from the following text, matching links to headings, if possible.\n\n" + stupidText);

        var chatService = kernel.GetRequiredService<IChatCompletionService>();

        var settings = new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.EnableFunctions([]) // intent: prevent recursion
        };

        var result = await chatService.GetChatMessageContentAsync(history, settings, kernel);

        Console.WriteLine("Summarizer > " + result.Content);

        return result.Content;
    }
}
