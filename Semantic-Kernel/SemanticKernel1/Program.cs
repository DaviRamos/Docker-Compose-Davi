using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.Ollama;
using SemanticKernel1.Plugins;

var builder = Kernel.CreateBuilder()
    .AddOllamaChatCompletion(
        modelId: "llama3.1:latest",
        endpoint: new Uri("http://localhost:11434")
    );        
 
//Enterprise Components
builder.Services.AddLogging(x 
    => x.AddConsole().SetMinimumLevel(LogLevel.Trace));

var kernel  = builder.Build();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

kernel.Plugins.AddFromType<ProductPlugin>("Plugins");

OllamaPromptExecutionSettings settings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

Console.WriteLine("Write your message to the AI bot!");

var history = new ChatHistory();
string? userInput;
while (true)
{
    userInput = Console.ReadLine();
    if (string.IsNullOrEmpty(userInput))
    {
        break;
    }

    history.AddUserMessage(userInput);

    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: settings,
        kernel: kernel);

    history.AddMessage(result.Role, result.Content ?? string.Empty);

    Console.WriteLine($"AI: {result.Content}");
}
