# Código Fonte Completo - NetRedisASide2

## 📁 Models/Assunto.cs

```csharp
namespace NetRedisASide2.Api.Models;

public class Assunto
{
    public int Id { get; set; }
    public string Nome { get; set; } = String.Empty;
    public string Descricao { get; set; } = String.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
```

---

## 📁 Models/Movimentacao.cs

```csharp
namespace NetRedisASide2.Api.Models;

public class Movimentacao
{
    public int Id { get; set; }
    public string Nome { get; set; } = String.Empty;
    public string Descricao { get; set; } = String.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
```

---

## 📁 Models/TipoDocumento.cs

```csharp
namespace NetRedisASide2.Api.Models;

public class TipoDocumento
{
    public int Id { get; set; }
    public string Nome { get; set; } = String.Empty;
    public string Descricao { get; set; } = String.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
```

---

## 📁 Data/AppDbContext.cs

```csharp
using Microsoft.EntityFrameworkCore;
using NetRedisASide2.Api.Models;

namespace NetRedisASide2.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Assunto> Assuntos { get; set; }
    public DbSet<Movimentacao> Movimentacoes { get; set; }
    public DbSet<TipoDocumento> TiposDocumento { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Assunto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.DataAtualizacao).IsRequired();
        });

        modelBuilder.Entity<Movimentacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.DataAtualizacao).IsRequired();
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.DataAtualizacao).IsRequired();
        });
    }
}
```

---

## 📁 Repositories/IRepository.cs

```csharp
namespace NetRedisASide2.Api.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
}
```

---

## 📁 Repositories/Repository.cs

```csharp
using Microsoft.EntityFrameworkCore;
using NetRedisASide2.Api.Data;

namespace NetRedisASide2.Api.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<T> UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity == null)
            return false;

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}
```

---

## 📁 Repositories/AssuntoRepository.cs

```csharp
using NetRedisASide2.Api.Data;
using NetRedisASide2.Api.Models;

namespace NetRedisASide2.Api.Repositories;

public interface IAssuntoRepository : IRepository<Assunto>
{
}

public class AssuntoRepository : Repository<Assunto>, IAssuntoRepository
{
    public AssuntoRepository(AppDbContext context) : base(context)
    {
    }
}
```

---

## 📁 Repositories/MovimentacaoRepository.cs

```csharp
using NetRedisASide2.Api.Data;
using NetRedisASide2.Api.Models;

namespace NetRedisASide2.Api.Repositories;

public interface IMovimentacaoRepository : IRepository<Movimentacao>
{
}

public class MovimentacaoRepository : Repository<Movimentacao>, IMovimentacaoRepository
{
    public MovimentacaoRepository(AppDbContext context) : base(context)
    {
    }
}
```

---

## 📁 Repositories/TipoDocumentoRepository.cs

```csharp
using NetRedisASide2.Api.Data;
using NetRedisASide2.Api.Models;

namespace NetRedisASide2.Api.Repositories;

public interface ITipoDocumentoRepository : IRepository<TipoDocumento>
{
}

public class TipoDocumentoRepository : Repository<TipoDocumento>, ITipoDocumentoRepository
{
    public TipoDocumentoRepository(AppDbContext context) : base(context)
    {
    }
}
```

---

## 📁 Services/AssuntoService.cs

```csharp
using Microsoft.Extensions.Caching.Distributed;
using NetRedisASide2.Api.Models;
using NetRedisASide2.Api.Repositories;
using System.Text.Json;

namespace NetRedisASide2.Api.Services;

public interface IAssuntoService
{
    Task<IEnumerable<Assunto>> GetAllAsync();
    Task<Assunto?> GetByIdAsync(int id);
    Task<Assunto> CreateAsync(Assunto assunto);
    Task<Assunto?> UpdateAsync(int id, Assunto assunto);
    Task<bool> DeleteAsync(int id);
}

public class AssuntoService : IAssuntoService
{
    private readonly IAssuntoRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly string _cacheKeyPrefix = "assunto:";

    public AssuntoService(IAssuntoRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<Assunto>> GetAllAsync()
    {
        var cacheKey = $"{_cacheKeyPrefix}all";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<Assunto>>(cachedData) ?? Enumerable.Empty<Assunto>();
        }

        var assuntos = await _repository.GetAllAsync();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(assuntos),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return assuntos;
    }

    public async Task<Assunto?> GetByIdAsync(int id)
    {
        var cacheKey = $"{_cacheKeyPrefix}{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<Assunto>(cachedData);
        }

        var assunto = await _repository.GetByIdAsync(id);
        if (assunto != null)
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(assunto),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        return assunto;
    }

    public async Task<Assunto> CreateAsync(Assunto assunto)
    {
        assunto.DataCriacao = DateTime.UtcNow;
        assunto.DataAtualizacao = DateTime.UtcNow;
        var created = await _repository.AddAsync(assunto);
        await InvalidateCache();
        return created;
    }

    public async Task<Assunto?> UpdateAsync(int id, Assunto assunto)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.Nome = assunto.Nome;
        existing.Descricao = assunto.Descricao;
        existing.DataAtualizacao = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        await InvalidateCache(id);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await InvalidateCache(id);
        }
        return result;
    }

    private async Task InvalidateCache(int? id = null)
    {
        await _cache.RemoveAsync($"{_cacheKeyPrefix}all");
        if (id.HasValue)
        {
            await _cache.RemoveAsync($"{_cacheKeyPrefix}{id}");
        }
    }
}
```

---

## 📁 Services/MovimentacaoService.cs

```csharp
using Microsoft.Extensions.Caching.Distributed;
using NetRedisASide2.Api.Models;
using NetRedisASide2.Api.Repositories;
using System.Text.Json;

namespace NetRedisASide2.Api.Services;

public interface IMovimentacaoService
{
    Task<IEnumerable<Movimentacao>> GetAllAsync();
    Task<Movimentacao?> GetByIdAsync(int id);
    Task<Movimentacao> CreateAsync(Movimentacao movimentacao);
    Task<Movimentacao?> UpdateAsync(int id, Movimentacao movimentacao);
    Task<bool> DeleteAsync(int id);
}

public class MovimentacaoService : IMovimentacaoService
{
    private readonly IMovimentacaoRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly string _cacheKeyPrefix = "movimentacao:";

    public MovimentacaoService(IMovimentacaoRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<Movimentacao>> GetAllAsync()
    {
        var cacheKey = $"{_cacheKeyPrefix}all";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<Movimentacao>>(cachedData) ?? Enumerable.Empty<Movimentacao>();
        }

        var movimentacoes = await _repository.GetAllAsync();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(movimentacoes),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return movimentacoes;
    }

    public async Task<Movimentacao?> GetByIdAsync(int id)
    {
        var cacheKey = $"{_cacheKeyPrefix}{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<Movimentacao>(cachedData);
        }

        var movimentacao = await _repository.GetByIdAsync(id);
        if (movimentacao != null)
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(movimentacao),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        return movimentacao;
    }

    public async Task<Movimentacao> CreateAsync(Movimentacao movimentacao)
    {
        movimentacao.DataCriacao = DateTime.UtcNow;
        movimentacao.DataAtualizacao = DateTime.UtcNow;
        var created = await _repository.AddAsync(movimentacao);
        await InvalidateCache();
        return created;
    }

    public async Task<Movimentacao?> UpdateAsync(int id, Movimentacao movimentacao)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.Nome = movimentacao.Nome;
        existing.Descricao = movimentacao.Descricao;
        existing.DataAtualizacao = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        await InvalidateCache(id);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await InvalidateCache(id);
        }
        return result;
    }

    private async Task InvalidateCache(int? id = null)
    {
        await _cache.RemoveAsync($"{_cacheKeyPrefix}all");
        if (id.HasValue)
        {
            await _cache.RemoveAsync($"{_cacheKeyPrefix}{id}");
        }
    }
}
```

---

## 📁 Services/TipoDocumentoService.cs

```csharp
using Microsoft.Extensions.Caching.Distributed;
using NetRedisASide2.Api.Models;
using NetRedisASide2.Api.Repositories;
using System.Text.Json;

namespace NetRedisASide2.Api.Services;

public interface ITipoDocumentoService
{
    Task<IEnumerable<TipoDocumento>> GetAllAsync();
    Task<TipoDocumento?> GetByIdAsync(int id);
    Task<TipoDocumento> CreateAsync(TipoDocumento tipoDocumento);
    Task<TipoDocumento?> UpdateAsync(int id, TipoDocumento tipoDocumento);
    Task<bool> DeleteAsync(int id);
}

public class TipoDocumentoService : ITipoDocumentoService
{
    private readonly ITipoDocumentoRepository _repository;
    private readonly IDistributedCache _cache;
    private readonly string _cacheKeyPrefix = "tipodocumento:";

    public TipoDocumentoService(ITipoDocumentoRepository repository, IDistributedCache cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<IEnumerable<TipoDocumento>> GetAllAsync()
    {
        var cacheKey = $"{_cacheKeyPrefix}all";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<IEnumerable<TipoDocumento>>(cachedData) ?? Enumerable.Empty<TipoDocumento>();
        }

        var tiposDocumento = await _repository.GetAllAsync();
        await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(tiposDocumento),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

        return tiposDocumento;
    }

    public async Task<TipoDocumento?> GetByIdAsync(int id)
    {
        var cacheKey = $"{_cacheKeyPrefix}{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cachedData))
        {
            return JsonSerializer.Deserialize<TipoDocumento>(cachedData);
        }

        var tipoDocumento = await _repository.GetByIdAsync(id);
        if (tipoDocumento != null)
        {
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(tipoDocumento),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
        }

        return tipoDocumento;
    }

    public async Task<TipoDocumento> CreateAsync(TipoDocumento tipoDocumento)
    {
        tipoDocumento.DataCriacao = DateTime.UtcNow;
        tipoDocumento.DataAtualizacao = DateTime.UtcNow;
        var created = await _repository.AddAsync(tipoDocumento);
        await InvalidateCache();
        return created;
    }

    public async Task<TipoDocumento?> UpdateAsync(int id, TipoDocumento tipoDocumento)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null)
            return null;

        existing.Nome = tipoDocumento.Nome;
        existing.Descricao = tipoDocumento.Descricao;
        existing.DataAtualizacao = DateTime.UtcNow;

        var updated = await _repository.UpdateAsync(existing);
        await InvalidateCache(id);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await InvalidateCache(id);
        }
        return result;
    }

    private async Task InvalidateCache(int? id = null)
    {
        await _cache.RemoveAsync($"{_cacheKeyPrefix}all");
        if (id.HasValue)
        {
            await _cache.RemoveAsync($"{_cacheKeyPrefix}{id}");
        }
    }
}
```

---

## 📁 Endpoints/AssuntoEndpoints.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using NetRedisASide2.Api.Models;
using NetRedisASide2.Api.Services;

namespace NetRedisASide2.Api.Endpoints;

public static class AssuntoEndpoints
{
    public static void MapAssuntoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assuntos")
            .WithTags("Assuntos")
            .RequireAuthorization();

        group.MapGet("/", async (IAssuntoService service) =>
        {
            var assuntos = await service.GetAllAsync();
            return Results.Ok(assuntos);
        })
        .WithName("GetAllAssuntos")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id, IAssuntoService service) =>
        {
            var assunto = await service.GetByIdAsync(id);
            return assunto is not null ? Results.Ok(assunto) : Results.NotFound();
        })
        .WithName("GetAssuntoById")
        .WithOpenApi();

        group.MapPost("/", async (Assunto assunto, IAssuntoService service) =>
        {
            var created = await service.CreateAsync(assunto);
            return Results.Created($"/api/assuntos/{created.Id}", created);
        })
        .WithName("CreateAssunto")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, Assunto assunto, IAssuntoService service) =>
        {
            var updated = await service.UpdateAsync(id, assunto);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        })
        .WithName("UpdateAssunto")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id, IAssuntoService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteAssunto")
        .WithOpenApi();
    }
}
```

---

## 📁 Endpoints/MovimentacaoEndpoints.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using NetRedisASide2.Api.Models;
using NetRedisASide2.Api.Services;

namespace NetRedisASide2.Api.Endpoints;

public static class MovimentacaoEndpoints
{
    public static void MapMovimentacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/movimentacoes")
            .WithTags("Movimentacoes")
            .RequireAuthorization();

        group.MapGet("/", async (IMovimentacaoService service) =>
        {
            var movimentacoes = await service.GetAllAsync();
            return Results.Ok(movimentacoes);
        })
        .WithName("GetAllMovimentacoes")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id, IMovimentacaoService service) =>
        {
            var movimentacao = await service.GetByIdAsync(id);
            return movimentacao is not null ? Results.Ok(movimentacao) : Results.NotFound();
        })
        .WithName("GetMovimentacaoById")
        .WithOpenApi();

        group.MapPost("/", async (Movimentacao movimentacao, IMovimentacaoService service) =>
        {
            var created = await service.CreateAsync(movimentacao);
            return Results.Created($"/api/movimentacoes/{created.Id}", created);
        })
        .WithName("CreateMovimentacao")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, Movimentacao movimentacao, IMovimentacaoService service) =>
        {
            var updated = await service.UpdateAsync(id, movimentacao);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        })
        .WithName("UpdateMovimentacao")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id, IMovimentacaoService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteMovimentacao")
        .WithOpenApi();
    }
}
```

---

## 📁 Endpoints/TipoDocumentoEndpoints.cs

```csharp
using Microsoft.AspNetCore.Authorization;
using NetRedisASide2.Api.Models;
using NetRedisASide2.Api.Services;

namespace NetRedisASide2.Api.Endpoints;

public static class TipoDocumentoEndpoints
{
    public static void MapTipoDocumentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tipos-documento")
            .WithTags("TiposDocumento")
            .RequireAuthorization();

        group.MapGet("/", async (ITipoDocumentoService service) =>
        {
            var tiposDocumento = await service.GetAllAsync();
            return Results.Ok(tiposDocumento);
        })
        .WithName("GetAllTiposDocumento")
        .WithOpenApi();

        group.MapGet("/{id}", async (int id, ITipoDocumentoService service) =>
        {
            var tipoDocumento = await service.GetByIdAsync(id);
            return tipoDocumento is not null ? Results.Ok(tipoDocumento) : Results.NotFound();
        })
        .WithName("GetTipoDocumentoById")
        .WithOpenApi();

        group.MapPost("/", async (TipoDocumento tipoDocumento, ITipoDocumentoService service) =>
        {
            var created = await service.CreateAsync(tipoDocumento);
            return Results.Created($"/api/tipos-documento/{created.Id}", created);
        })
        .WithName("CreateTipoDocumento")
        .WithOpenApi();

        group.MapPut("/{id}", async (int id, TipoDocumento tipoDocumento, ITipoDocumentoService service) =>
        {
            var updated = await service.UpdateAsync(id, tipoDocumento);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        })
        .WithName("UpdateTipoDocumento")
        .WithOpenApi();

        group.MapDelete("/{id}", async (int id, ITipoDocumentoService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTipoDocumento")
        .WithOpenApi();
    }
}
```

---

## 📁 Extensions/ServiceCollectionExtensions.cs

```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NetRedisASide2.Api.Data;
using NetRedisASide2.Api.Repositories;
using NetRedisASide2.Api.Services;

namespace NetRedisASide2.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Redis Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Redis");
            options.InstanceName = "NetRedisASide2:";
        });

        // Repositories
        services.AddScoped<IAssuntoRepository, AssuntoRepository>();
        services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
        services.AddScoped<ITipoDocumentoRepository, TipoDocumentoRepository>();

        // Services
        services.AddScoped<IAssuntoService, AssuntoService>();
        services.AddScoped<IMovimentacaoService, MovimentacaoService>();
        services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();

        // Authentication
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var keycloakUrl = configuration["Keycloak:Url"];
                var realm = configuration["Keycloak:Realm"];
                
                options.Authority = $"{keycloakUrl}/realms/{realm}";
                options.Audience = configuration["Keycloak:ClientId"];
                options.RequireHttpsMetadata = false;
                
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true
                };
            });

        services.AddAuthorization();

        return services;
    }
}
```

---

## 📁 Extensions/WebApplicationExtensions.cs

```csharp
using NetRedisASide2.Api.Endpoints;

namespace NetRedisASide2.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MapApplicationEndpoints(this WebApplication app)
    {
        app.MapAssuntoEndpoints();
        app.MapMovimentacaoEndpoints();
        app.MapTipoDocumentoEndpoints();

        return app;
    }
}
```

---

## 📁 Program.cs

```csharp
using NetRedisASide2.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapApplicationEndpoints();

app.Run();
```

---

## 📁 NetRedisASide2.Api.csproj

```xml
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
      <PrivateAssets>all</PrivateAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
    <PackageReference Include="StackExchange.Redis" Version="2.7.10" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>

</Project>
```

---

## 📁 appsettings.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Host=${POSTGRES_HOST};Port=${POSTGRES_PORT};Database=${POSTGRES_DB};Username=${POSTGRES_USER};Password=${POSTGRES_PASSWORD}",
    "Redis": "${REDIS_HOST}:${REDIS_PORT}"
  },
  "Keycloak": {
    "Url": "${KEYCLOAK_URL}",
    "Realm": "${KEYCLOAK_REALM}",
    "ClientId": "${KEYCLOAK_CLIENT_ID}",
    "ClientSecret": "${KEYCLOAK_CLIENT_SECRET}"
  }
}
```

---

## 📁 appsettings.Development.json

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=netredisaside2_db;Username=netredisaside2_user;Password=netredisaside2_pass",
    "Redis": "localhost:6379"
  },
  "Keycloak": {
    "Url": "http://localhost:8080",
    "Realm": "NetRedisASide2",
    "ClientId": "netredisaside2-api",
    "ClientSecret": "your-client-secret-here"
  }
}
```

---

## 🎯 Resumo da Arquitetura

### Camadas do Aplicativo:

1. **Models**: Entidades de domínio
2. **Data**: DbContext do Entity Framework
3. **Repositories**: Acesso a dados com padrão Repository
4. **Services**: Lógica de negócio + Cache Redis
5. **Endpoints**: Minimal API endpoints
6. **Extensions**: Configuração de DI e middleware

### Características Implementadas:

✅ **CRUD Completo** para 3 entidades  
✅ **Cache Redis** com invalidação automática  
✅ **Autenticação JWT** via Keycloak  
✅ **Padrão Repository** genérico  
✅ **Dependency Injection** completa  
✅ **Minimal API** com grupos e tags  
✅ **Entity Framework Core** com PostgreSQL  
✅ **Swagger/OpenAPI** integrado  

### Fluxo de Requisição:

```
HTTP Request 
  → Endpoint (Authorization check)
  → Service (Cache check → Repository)
  → Repository (Database)
  → Response (com cache)
```
