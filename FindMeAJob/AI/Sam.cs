using FindMeAJob.AI.Plugins;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace FindMeAJob.AI;

public sealed class Sam: IHostedService
{
    private Kernel Kernel { get; }
    private IChatCompletionService ChatService { get; }
    private ChatHistory History { get; } = [];

    private OpenAIPromptExecutionSettings Settings { get; } = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    private static readonly string[] AttentionPrefixes = [ "BEN", "**BEN**" ];

    public Sam(Kernel kernel, IOptions<SamOptions> options)
    {
        Kernel = kernel;
        ChatService = kernel.GetRequiredService<IChatCompletionService>();

        kernel.Plugins.AddFromType<WebPageJobSummariesPlugin>();
        //kernel.Plugins.AddFromType<WebPageLinksPlugin>(); // commented out, because Sam seemed to get stuck on it a lot

        // should this all be in config? ... maybe >_>
        History.AddSystemMessage($"""
            Your name is Sam. You are an AI written by Ben to help Ben find a job.
            This is how Ben has described what he's looking for: {options.Value.JobToFind}
            Please use the methods available to you to locate jobs. When you find a good match, give your recommendation to Ben by including a link to the job posting (so he can apply!), the job title, and - most importantly - a letter to the company, from you, that starts by stating who and what YOU are (including a link to your source code: https://github.com/BenMakesGames/Sam), your relationship to Ben, how you found the job posting, and why you think Ben would be a good fit for the job based on the job's description.
            Here are some URLs to help get you started: {string.Join(", ", options.Value.UrlsToGetStarted)}
            If you need Ben to personally respond to your message, start that message with the text "BEN".
        """);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        // should this all be in config? ... ALMOST CERTAINLY >_>
        const string bensIntro = "Hey, Sam, this is Ben! Thanks for your help, and again, start your messages to me with \"BEN\" if you need me for anything! Thanks!";

        Console.WriteLine($"Ben > {bensIntro}");
        History.AddUserMessage(bensIntro);

        while (true)
        {
            var result = ChatService.GetStreamingChatMessageContentsAsync(History, Settings, Kernel, cancellationToken);

            var fullMessage = "";
            var first = true;

            await foreach (var content in result)
            {
                if (content.Role.HasValue && first)
                {
                    Console.Write("Sam > ");
                    first = false;
                }

                Console.Write(content.Content);
                fullMessage += content.Content;
            }

            Console.WriteLine();
            History.AddAssistantMessage(fullMessage);

            if (AttentionPrefixes.Any(w => fullMessage.StartsWith(w, StringComparison.InvariantCultureIgnoreCase)))
            {
                History.AddAssistantMessage(fullMessage);

                while (true)
                {
                    Console.Write("Ben > ");
                    var response = Console.ReadLine()?.Trim();

                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        if (response.ToLowerInvariant() == "quit" || response.ToLowerInvariant() == "exit")
                            return;

                        Console.WriteLine("Confirm the above message by typing \"yes\".");
                        if (Console.ReadLine()?.Trim().ToLowerInvariant() == "yes")
                        {
                            History.AddUserMessage(response);
                            break;
                        }
                    }
                }
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StopAsync(...) is not implemented.");

        return Task.CompletedTask;
    }
}

public sealed class SamOptions
{
    public required string JobToFind { get; set; }
    public required IList<string> UrlsToGetStarted { get; set; }
}
