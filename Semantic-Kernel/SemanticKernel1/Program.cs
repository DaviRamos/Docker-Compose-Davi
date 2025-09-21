using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.Ollama;

var builder = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(
        modelid: "llama3.1:latest",
        endpoint: new Uri("http://localhost:11434")
        )
    .Build();

//Enterprise Components
builder.Services.AddLogging(x 
    => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

var app = builder.Build();

var chatCompletionService = app.GetRequiredService<IChatCompletion>();

string? input;
do
{
    Console.Write("User > ");
    input = Console.ReadLine();

    var result = chatCompletionService
    .GetChatMessagesAsync(input, Kernel: app);

    Console.WriteLine("Assistant > " + result);
    
} while (input is null);
