using CoffeeShop.Components;
using CoffeeShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? throw new Exception("Connection string not found");

var ollamaUrl = builder.Configuration.GetValue<string>("OllamaUrl")
                ?? throw new Exception("Ollama URL not found");

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlite(connectionString));

builder.Services.AddOllamaChatCompletion(
    modelId: "phi3",
    endpoint: new Uri(ollamaUrl)
);

builder.Services.AddTransient((serviceProvider) => new Kernel(serviceProvider));

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