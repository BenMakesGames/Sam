using System.ComponentModel;
using Microsoft.SemanticKernel;

namespace FindMeAJob.AI.Plugins;

public class WebPlugin
{
    private const int MinDelayBetweenWebRequestsInSeconds = 2;

    private long LastGetPageContentsCall { get; set; } = 0;

    [KernelFunction]
    [Description("gets the contents of a page, given its URL")]
    public async Task<string> GetPageContents(string url)
    {
        // wait at least 2 seconds between page calls
        var ticksSinceLastCall = DateTime.Now.Ticks - LastGetPageContentsCall;

        if (ticksSinceLastCall < MinDelayBetweenWebRequestsInSeconds * TimeSpan.TicksPerSecond)
            await Task.Delay((int)((MinDelayBetweenWebRequestsInSeconds * TimeSpan.TicksPerSecond - ticksSinceLastCall) / TimeSpan.TicksPerMillisecond));

        using var client = new HttpClient();

        var response = await client.GetStringAsync(url);

        LastGetPageContentsCall = DateTime.Now.Ticks;

        return response;
    }
}
