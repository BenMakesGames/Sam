using FindMeAJob;
using FindMeAJob.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.SemanticKernel;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Sam>();

builder.Services.Configure<SamOptions>(builder.Configuration.GetSection("Sam"));
builder.Services.AddSingleton<Sam>();

var openAiOptions = builder.Configuration.GetSection("OpenAI").Get<OpenAIOptions>()
    ?? throw new Exception("OpenAI configuration is missing.");

builder.Services.AddKernel().AddOpenAIChatCompletion(openAiOptions.ModelName, openAiOptions.ModelName);

var app = builder.Build();

Console.WriteLine("Start!");

await app.StartAsync();
