using System.Xml;
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
    private IChatCompletionService Brain { get; }
    private ChatHistory History { get; } = [];

    public Sam(Kernel kernel, IOptions<SamOptions> options)
    {
        Kernel = kernel;
        Brain = kernel.GetRequiredService<IChatCompletionService>();

        kernel.Plugins.AddFromType<WebPlugin>();

        History.AddSystemMessage(new("Your name is Sam, and you're helping Ben find a job."));
        History.AddSystemMessage(new($"This is how Ben has described what he's looking for: {options.Value.JobToFind}"));
        History.AddSystemMessage("Please use the methods available to you to locate jobs, and give your recommendations to Ben. Your recommendation should include a link, job title, why you think it's a good for Ben, and most importantly, a proposed cover letter for the job that starts by stating who you are, what you're doing, and why you think Ben would be a good fit for the job.");
        History.AddSystemMessage($"Here are some URLs to help get you started: {string.Join(", ", options.Value.UrlsToGetStarted)}");
        History.AddSystemMessage("Please structure your responses as follows: start your message with \"JOB\" if it's about a job you found; Ben will get back to you with his thoughts on the job. Start your message with \"BEN\" if you have a general question for Ben: a request for more information, changes to your functionality, etc. All other messages will be ignored, but you can use these responses to keep notes for yourself, etc.");
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        const string bensIntro = "Hey, this is Ben! Thanks for your help, and again, if you need anything from me, start your message with \"BEN\", and I'll get back to you! Thanks!";

        Console.WriteLine($"Ben > {bensIntro}");
        History.AddUserMessage(bensIntro);

        var settings = new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };

        while (true)
        {
            var result = Brain.GetStreamingChatMessageContentsAsync(
                History,
                executionSettings: settings,
                kernel: Kernel,
                cancellationToken: cancellationToken);

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
                fullMessage += fullMessage;
            }

            Console.WriteLine();
            History.AddAssistantMessage(fullMessage);

            if (
                fullMessage.StartsWith("BEN", StringComparison.InvariantCultureIgnoreCase) ||
                fullMessage.StartsWith("JOB", StringComparison.InvariantCultureIgnoreCase)
            )
            {
                while (true)
                {
                    Console.Write("Ben > ");
                    var response = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(response))
                    {
                        Console.WriteLine("Reply with that message? Type \"yes\" to confirm.");
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
