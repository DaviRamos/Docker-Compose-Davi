using Microsoft.Extensions.AI;
using OpenAI.Chat;

const string openAIKey = "Hello, World!";
var ollamUrl = new Uri("http://localhost:11434");

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var cache = new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions()));

var client = app.Environment.IsDevelopment()
    ? new OllamaClient(ollamUrl, "phi3:latest")
    : new OpenAI.Chat.ChatClient("gpt-40-mini", openAIKey)
        .AsIChatClient();

var cachedClient = new ChatClientBuilder(client)
    .UseDistributedCache(cache)
    .Build();

app.MapPost("/", async (Question question) =>
{
    var result = await cachedClient.GetResponseAsync(question.Prompt);
    return Results.Ok(result.Text);
});


app.MapPost("/v2", async (Question question) =>
{
    var result = await client.GetResponseAsync(
    [
        new ChatMessage(ChatRole.System,
        "You're weather expert,answer me in ust one setences, whitin 50 words"),

            new ChatMessage(ChatRole.User, question.Prompt)
        ]
    );
    return Results.Ok(result.Text);
});

app.MapPost("/v3", async(Question question) =>
{
    var result = await cachedClient.GetResponseAsync(
    [
        new ChatMessage(ChatRole.System,
        "You're weather expert,answer me in ust one setences, whitin 50 words"),

            new ChatMessage(ChatRole.User, question.Prompt)
        ]
    );
    return Results.Ok(result.Text); 
});


app.Run();

public record Question(string Prompt);