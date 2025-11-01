using Microsoft.EntityFrameworkCore;
using Poststore.Data;
using Poststore.Models;
using Poststore.Requests;

var builder = WebApplication.CreateBuilder(args);

var cnnStr =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("Connection string not found");
builder.Services.AddDbContext<AppDbContext>(x
    => x.UseNpgsql(cnnStr));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

// app.MapPost("/v1/categories", async (AppDbContext context, Category category) =>
// {
//     await context.Categories.AddAsync(category);
//     await context.SaveChangesAsync();
//     return Results.Created();
// });

app.MapPost("/v1/categories", async (AppDbContext context, CreateCategoryRequest request) =>
{
    var category = new Category
    {
        Heading = new Heading
        {
            Title = request.Title,
            Slug = request.Slug
        }
    };
    await context.Categories.AddAsync(category);
    await context.SaveChangesAsync();
    return Results.Created();
});

app.MapGet("/v1/categories", async (AppDbContext context) =>
{
    var products = await context
        .Categories
        .AsNoTracking()
        .ToListAsync();
    return Results.Ok(products);
});

app.MapPost("/v1/products", async (AppDbContext context, Product product) =>
{
    await context.Products.AddAsync(product);
    await context.SaveChangesAsync();
    return Results.Created();
});

app.MapGet("/v1/products", async (AppDbContext context) =>
{
    var products = await context
        .Products
        .AsNoTracking()
        .ToListAsync();
    return Results.Ok(products);
});

app.Run();