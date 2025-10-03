using EfPostgresStore.Data;
using EfPostgresStore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

/*
builder.Services.AddTransient<OllamaAApiClient>(x =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("http://localhost:11434/"),
        Timeout = TimeSpan.FromMinutes(10)
    };
    return new OllamaAApiClient(httpClient);
}     );
*/

var app = builder.Build();

app.MapPost("v1/categories", async (
    AppDbContext context,
    Category category) =>
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        return Results.Created($"/v1/categories/{category.Id}", category);
    });
    

app.MapPut("v1/categories/{id}", async (
    AppDbContext context,
    int id,
    Category category) =>
    {
        var existingCategory = await context.Categories.FindAsync(id);
        if (existingCategory is null) return Results.NotFound();

        existingCategory.Title = category.Title;
        existingCategory.Slug = category.Slug;

        context.Categories.Update(existingCategory);
        await context.SaveChangesAsync();

        return Results.Ok(existingCategory);
    });

app.MapDelete("v1/categories/{id}", async (
    AppDbContext context,
    int id) =>
    {
        var existingCategory = await context.Categories.FindAsync(id);
        if (existingCategory is null) return Results.NotFound();

        context.Categories.Remove(existingCategory);        
        await context.SaveChangesAsync();

        return Results.Ok(existingCategory);
    });

app.MapGet("v1/categories", async (
    AppDbContext context ) =>
    {
        var categories = await context.Categories.AsNoTracking().ToListAsync();
        return Results.Ok(categories);
    });

app.MapGet("v1/categories/{id}", async (
    AppDbContext context,
    int id) =>
    {
        var existingCategory = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
    
        if (existingCategory is null) return Results.NotFound();
    
        return Results.Ok(existingCategory);
    });


app.MapGet("/",() => "Hello World!");

app.Run();
