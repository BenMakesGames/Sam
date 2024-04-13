using FindMeAJob;
using FindMeAJob.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

// main AI, Sam:
builder.Services.Configure<SamOptions>(builder.Configuration.GetSection("Sam"));
builder.Services.AddHostedService<Sam>();

var openAiOptions = builder.Configuration.GetSection("OpenAI").Get<OpenAIOptions>()
    ?? throw new Exception("OpenAI configuration is missing.");

// reminder: Kernels are transient!
builder.Services.AddKernel().AddOpenAIChatCompletion(openAiOptions.ModelName, openAiOptions.ApiKey);

var app = builder.Build();

// let's-a-go!
Console.WriteLine("Wahoo!");

await app.StartAsync();
