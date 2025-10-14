using NetRedisASide.Data;
using NetRedisASide.Models;
using NetRedisASide.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.StackExchangeRedis;


var builder = WebApplication.CreateBuilder(args);

// Add DbContext and other services 
var connectionStringPostgres = builder.Configuration.GetConnectionString("PostgresConnection")
    ?? throw new InvalidOperationException("Connection string 'PostgresConnection' not found.");

var connectionStringRedis = builder.Configuration.GetConnectionString("RedisConnection")
    ?? throw new InvalidOperationException("Connection string 'RedisConnection' not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionStringPostgres));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString(connectionStringRedis);
    options.InstanceName = "CacheInstance";
});

// Register your custom seeder class
builder.Services.AddScoped<DataSeeder>();

// Add controllers, swagger, etc.
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

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
    await seeder.SeedAsync();
}

app.UseSwagger();
app.UseSwaggerUI();

app.MapAssuntoEndpoints();
app.MapTipoDocumentoEndpoints();
app.MapMovimentacaoEndpoints();

app.MapGet("/", () => "Hello World!");

app.Run();
