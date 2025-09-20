using System.Xml.Schema;
using OllamaSharp;

var url = new Uri("http://localhost:11434");
var client = new OllamaApiClient(url);

var models = await client.ListLocalModelsAsync();
foreach (var model in models)
{
    Console.WriteLine(model.Name);
}

//await client.PullModelAsync("phi3:latest");

client.SelectedModel = "phi3:latest";

var chat = new Chat(client);
var prompt = Console.ReadLine() ?? string.Empty;

await foreach (var answer in chat.SendAsync(prompt))
    Console.Write(answer);

Console.WriteLine("");
Console.WriteLine("--");