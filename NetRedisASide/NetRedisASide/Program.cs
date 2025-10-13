using NetRedisASide.Data;
using NetRedisASide.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "NetRedisASide",
        Description = "Teste Redis + Postgress",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("v1/Assuntos", async (
    AppDbContext context,
    Assunto assunto) =>
    {
        await context.Assuntos.AddAsync(assunto);
        await context.SaveChangesAsync();

        return Results.Created($"/v1/assuntos/{assunto.Id}", assunto);
    });
    

app.MapPut("v1/assuntos/{id}", async (
    AppDbContext context,
    int id,
    Assunto assunto) =>
    {
        var existingAssunto = await context.Assuntos.FindAsync(id);
        if (existingAssunto is null) return Results.NotFound();

        existingAssunto.Nome = assunto.Nome;
        existingAssunto.Descricao = assunto.Descricao;

        context.Assuntos.Update(existingAssunto);
        await context.SaveChangesAsync();

        return Results.Ok(existingAssunto);
    });

app.MapDelete("v1/assuntos/{id}", async (
    AppDbContext context,
    int id) =>
    {
        var existingAssunto = await context.Assuntos.FindAsync(id);
        if (existingAssunto is null) return Results.NotFound();

        context.Assuntos.Remove(existingAssunto);
        await context.SaveChangesAsync();

        return Results.Ok(existingAssunto);
    });

app.MapGet("v1/assuntos", async (
    AppDbContext context ) =>
    {
        var assuntos = await context.Assuntos.AsNoTracking().ToListAsync();
        return Results.Ok(assuntos);
    });

app.MapGet("v1/assuntos/{id}", async (
    AppDbContext context,
    int id) =>
    {
        var existingAssunto = await context.Assuntos.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);

        if (existingAssunto is null) return Results.NotFound();

        return Results.Ok(existingAssunto);
    });


app.MapGet("/",() => "Hello World!");

app.Run();
