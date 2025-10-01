using EfPostgresStore.Data;
using EfPostgresStore.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

app.MapPost("v1/Categories", async(
    AppDbContext context,
    Category category) =>
    {
        await context.Categories.AddAsync(category);
        await context.SaveChangesAsync();

        return Results.Created($"/v1/Categories/{category.Id}", category);
    }
))

app.Run();
