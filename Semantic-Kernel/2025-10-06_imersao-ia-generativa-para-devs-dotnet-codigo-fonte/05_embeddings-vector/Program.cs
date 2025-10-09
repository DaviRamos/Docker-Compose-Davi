using CoffeeShop.Components;
using CoffeeShop.Data;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.SqliteVec;
using Microsoft.SemanticKernel.Embeddings;
using Microsoft.SemanticKernel.Connectors.Ollama;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new Exception("Connection string not found");

var ollamaUrl = builder.Configuration.GetValue<string>("OllamaUrl")
                ?? throw new Exception("Ollama URL not found");

const string chatModelId = "phi3";
const string embeddingModelId = "mxbai-embed-large";

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));

builder.Services.AddOllamaChatCompletion(
    modelId: chatModelId,
    endpoint: new Uri(ollamaUrl)
);

// var ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "mxbai-embed-large");
// var skEmbeddingSvc = ollama.AsEmbeddingGenerationService();
// // var aiEmbeddingGen = skEmbeddingSvc.AsEmbeddingGenerator();
// var store = builder.Services.BuildServiceProvider().GetRequiredService<SqliteVectorStore>();
// var products = store.GetCollection<long, ProductModel>("Products");
// await products.EnsureCollectionExistsAsync();


// var ollama = new OllamaApiClient(new Uri("http://localhost:11434"), "mxbai-embed-large");
// var embeddingGen = ollama.AsEmbeddingGenerationService().AsEmbeddingGenerator();
//
// var store = new SqliteVectorStore(connectionString, new SqliteVectorStoreOptions
// {
//     EmbeddingGenerator = embeddingGen
// });
//
// var products = store.GetCollection<int, ProductModel>("Products");
// await products.EnsureCollectionExistsAsync();

builder.Services.AddTransient(serviceProvider => new Kernel(serviceProvider));

var ollamaClient = new OllamaApiClient(
    uriString: ollamaUrl,
    defaultModel: embeddingModelId
);

builder.Services.AddTransient<OllamaApiClient>(x => ollamaClient);
builder.Services.AddSqliteVectorStore(
    _ => connectionString,
    _ => new SqliteVectorStoreOptions
    {
        EmbeddingGenerator = ollamaClient.AsEmbeddingGenerationService().AsEmbeddingGenerator()
    });
builder.Services.AddSingleton<SqliteCollection<int, ProductModel>>(x =>
{
    var store = x.GetRequiredService<SqliteVectorStore>();
    var products = store.GetCollection<int, ProductModel>("Products");
    products.EnsureCollectionExistsAsync().GetAwaiter().GetResult();
    return products;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();