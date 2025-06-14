using Microsoft.OpenApi.Models;
using Microsoft.Extensions.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using projetoTeste.Models;
using projetoTeste.Data;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore.Storage;


var builder = WebApplication.CreateBuilder(args);

// Add Logging
//builder.Logging.ClearProviders();
//builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddDbContext<AppDbContext>(
    options=>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
);


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Api de Predição de Preços",
        Description = "Uma ML.NET Web API para Predição de Preços de imóveis",
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

using (var serviceScope = app.Services.CreateScope())
{    
    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.EnsureCreatedAsync();  
}

/*using (var serviceScope = app.Services.CreateScope())
{    
    var context = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbInitializer.Initialize(context);  
}*/

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "V1");
        options.RoutePrefix = string.Empty;  // Sem Swagger/index.html
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
