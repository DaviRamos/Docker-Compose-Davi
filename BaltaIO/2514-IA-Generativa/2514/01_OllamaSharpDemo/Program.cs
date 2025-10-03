using OllamaSharp;

var uri = new Uri("http://localhost:11434");
var ollama = new OllamaApiClient(uri);

var models = await ollama.ListLocalModelsAsync();
foreach (var model in models)
    Console.WriteLine(model.Name);

// https://ollama.com/library
ollama.SelectedModel = "phi3:latest";

// await foreach (var status in ollama.PullModelAsync("llama3.1:405b"))
//     Console.WriteLine($"{status.Percent}% {status.Status}");

// await foreach (var stream in ollama.GenerateAsync("How are you today?"))
//     Console.Write(stream.Response);

var chat = new Chat(ollama);


var message = Console.ReadLine();
await foreach (var answerToken in chat.SendAsync(message))
    Console.Write(answerToken);

Console.WriteLine();
Console.WriteLine("--");