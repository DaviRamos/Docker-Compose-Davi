# 💻 Código Fonte Completo - NetRedisASide2 (.NET 9)

## 📁 MODELS (3 arquivos)

### Models/Assunto.cs
```csharp
namespace NetRedisASide2.Api.Models;

public class Assunto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
```

### Models/Movimentacao.cs
```csharp
namespace NetRedisASide2.Api.Models;

public class Movimentacao
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
```

### Models/TipoDocumento.cs
```csharp
namespace NetRedisASide2.Api.Models;

public class TipoDocumento
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public DateTime DataCriacao { get; set; }
    public DateTime DataAtualizacao { get; set; }
}
```

---

## 📁 DATA (1 arquivo)

### Data/AppDbContext.cs
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
            entity.HasIndex(e => e.Nome);
        });

        modelBuilder.Entity<Movimentacao>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.DataAtualizacao).IsRequired();
            entity.HasIndex(e => e.Nome);
        });

        modelBuilder.Entity<TipoDocumento>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descricao).HasMaxLength(1000);
            entity.Property(e => e.DataCriacao).IsRequired();
            entity.Property(e => e.DataAtualizacao).IsRequired();
            entity.HasIndex(e => e.Nome);
        });
    }
}
```

---

## 📁 REPOSITORIES (6 arquivos)

### Repositories/IRepository.cs
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

### Repositories/Repository.cs
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
        return await _dbSet.AsNoTracking().ToListAsync();
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

### Repositories/AssuntoRepository.cs
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

### Repositories/MovimentacaoRepository.cs
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

### Repositories/TipoDocumentoRepository.cs
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

## 📁 SERVICES (3 arquivos)

### Services/AssuntoService.cs
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
    private readonly ILogger<AssuntoService> _logger;
    private const string CacheKeyPrefix = "assunto:";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public AssuntoService(
        IAssuntoRepository repository, 
        IDistributedCache cache,
        ILogger<AssuntoService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Assunto>> GetAllAsync()
    {
        var cacheKey = $"{CacheKeyPrefix}all";
        
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retornando assuntos do cache");
                return JsonSerializer.Deserialize<IEnumerable<Assunto>>(cachedData) 
                    ?? Enumerable.Empty<Assunto>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao acessar cache, consultando banco de dados");
        }

        var assuntos = await _repository.GetAllAsync();
        
        try
        {
            await _cache.SetStringAsync(
                cacheKey, 
                JsonSerializer.Serialize(assuntos),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao salvar no cache");
        }

        return assuntos;
    }

    public async Task<Assunto?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retornando assunto {Id} do cache", id);
                return JsonSerializer.Deserialize<Assunto>(cachedData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao acessar cache para assunto {Id}", id);
        }

        var assunto = await _repository.GetByIdAsync(id);
        if (assunto != null)
        {
            try
            {
                await _cache.SetStringAsync(
                    cacheKey, 
                    JsonSerializer.Serialize(assunto),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheExpiration
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao salvar assunto {Id} no cache", id);
            }
        }

        return assunto;
    }

    public async Task<Assunto> CreateAsync(Assunto assunto)
    {
        assunto.DataCriacao = DateTime.UtcNow;
        assunto.DataAtualizacao = DateTime.UtcNow;
        
        var created = await _repository.AddAsync(assunto);
        await InvalidateCacheAsync();
        
        _logger.LogInformation("Assunto {Id} criado com sucesso", created.Id);
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
        await InvalidateCacheAsync(id);
        
        _logger.LogInformation("Assunto {Id} atualizado com sucesso", id);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await InvalidateCacheAsync(id);
            _logger.LogInformation("Assunto {Id} excluído com sucesso", id);
        }
        return result;
    }

    private async Task InvalidateCacheAsync(int? id = null)
    {
        try
        {
            await _cache.RemoveAsync($"{CacheKeyPrefix}all");
            if (id.HasValue)
            {
                await _cache.RemoveAsync($"{CacheKeyPrefix}{id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao invalidar cache");
        }
    }
}
```

### Services/MovimentacaoService.cs
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
    private readonly ILogger<MovimentacaoService> _logger;
    private const string CacheKeyPrefix = "movimentacao:";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public MovimentacaoService(
        IMovimentacaoRepository repository, 
        IDistributedCache cache,
        ILogger<MovimentacaoService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Movimentacao>> GetAllAsync()
    {
        var cacheKey = $"{CacheKeyPrefix}all";
        
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retornando movimentações do cache");
                return JsonSerializer.Deserialize<IEnumerable<Movimentacao>>(cachedData) 
                    ?? Enumerable.Empty<Movimentacao>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao acessar cache, consultando banco de dados");
        }

        var movimentacoes = await _repository.GetAllAsync();
        
        try
        {
            await _cache.SetStringAsync(
                cacheKey, 
                JsonSerializer.Serialize(movimentacoes),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao salvar no cache");
        }

        return movimentacoes;
    }

    public async Task<Movimentacao?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retornando movimentação {Id} do cache", id);
                return JsonSerializer.Deserialize<Movimentacao>(cachedData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao acessar cache para movimentação {Id}", id);
        }

        var movimentacao = await _repository.GetByIdAsync(id);
        if (movimentacao != null)
        {
            try
            {
                await _cache.SetStringAsync(
                    cacheKey, 
                    JsonSerializer.Serialize(movimentacao),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheExpiration
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao salvar movimentação {Id} no cache", id);
            }
        }

        return movimentacao;
    }

    public async Task<Movimentacao> CreateAsync(Movimentacao movimentacao)
    {
        movimentacao.DataCriacao = DateTime.UtcNow;
        movimentacao.DataAtualizacao = DateTime.UtcNow;
        
        var created = await _repository.AddAsync(movimentacao);
        await InvalidateCacheAsync();
        
        _logger.LogInformation("Movimentação {Id} criada com sucesso", created.Id);
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
        await InvalidateCacheAsync(id);
        
        _logger.LogInformation("Movimentação {Id} atualizada com sucesso", id);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await InvalidateCacheAsync(id);
            _logger.LogInformation("Movimentação {Id} excluída com sucesso", id);
        }
        return result;
    }

    private async Task InvalidateCacheAsync(int? id = null)
    {
        try
        {
            await _cache.RemoveAsync($"{CacheKeyPrefix}all");
            if (id.HasValue)
            {
                await _cache.RemoveAsync($"{CacheKeyPrefix}{id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao invalidar cache");
        }
    }
}
```

### Services/TipoDocumentoService.cs
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
    private readonly ILogger<TipoDocumentoService> _logger;
    private const string CacheKeyPrefix = "tipodocumento:";
    private static readonly TimeSpan CacheExpiration = TimeSpan.FromMinutes(10);

    public TipoDocumentoService(
        ITipoDocumentoRepository repository, 
        IDistributedCache cache,
        ILogger<TipoDocumentoService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<TipoDocumento>> GetAllAsync()
    {
        var cacheKey = $"{CacheKeyPrefix}all";
        
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retornando tipos de documento do cache");
                return JsonSerializer.Deserialize<IEnumerable<TipoDocumento>>(cachedData) 
                    ?? Enumerable.Empty<TipoDocumento>();
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao acessar cache, consultando banco de dados");
        }

        var tiposDocumento = await _repository.GetAllAsync();
        
        try
        {
            await _cache.SetStringAsync(
                cacheKey, 
                JsonSerializer.Serialize(tiposDocumento),
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = CacheExpiration
                });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao salvar no cache");
        }

        return tiposDocumento;
    }

    public async Task<TipoDocumento?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        
        try
        {
            var cachedData = await _cache.GetStringAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedData))
            {
                _logger.LogInformation("Retornando tipo de documento {Id} do cache", id);
                return JsonSerializer.Deserialize<TipoDocumento>(cachedData);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao acessar cache para tipo de documento {Id}", id);
        }

        var tipoDocumento = await _repository.GetByIdAsync(id);
        if (tipoDocumento != null)
        {
            try
            {
                await _cache.SetStringAsync(
                    cacheKey, 
                    JsonSerializer.Serialize(tipoDocumento),
                    new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = CacheExpiration
                    });
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Erro ao salvar tipo de documento {Id} no cache", id);
            }
        }

        return tipoDocumento;
    }

    public async Task<TipoDocumento> CreateAsync(TipoDocumento tipoDocumento)
    {
        tipoDocumento.DataCriacao = DateTime.UtcNow;
        tipoDocumento.DataAtualizacao = DateTime.UtcNow;
        
        var created = await _repository.AddAsync(tipoDocumento);
        await InvalidateCacheAsync();
        
        _logger.LogInformation("Tipo de documento {Id} criado com sucesso", created.Id);
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
        await InvalidateCacheAsync(id);
        
        _logger.LogInformation("Tipo de documento {Id} atualizado com sucesso", id);
        return updated;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        if (result)
        {
            await InvalidateCacheAsync(id);
            _logger.LogInformation("Tipo de documento {Id} excluído com sucesso", id);
        }
        return result;
    }

    private async Task InvalidateCacheAsync(int? id = null)
    {
        try
        {
            await _cache.RemoveAsync($"{CacheKeyPrefix}all");
            if (id.HasValue)
            {
                await _cache.RemoveAsync($"{CacheKeyPrefix}{id}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Erro ao invalidar cache");
        }
    }
}
```

---

## 📁 ENDPOINTS (3 arquivos)

### Endpoints/AssuntoEndpoints.cs
```csharp
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
        .WithOpenApi()
        .Produces<IEnumerable<Assunto>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, IAssuntoService service) =>
        {
            var assunto = await service.GetByIdAsync(id);
            return assunto is not null ? Results.Ok(assunto) : Results.NotFound();
        })
        .WithName("GetAssuntoById")
        .WithOpenApi()
        .Produces<Assunto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (Assunto assunto, IAssuntoService service) =>
        {
            var created = await service.CreateAsync(assunto);
            return Results.Created($"/api/assuntos/{created.Id}", created);
        })
        .WithName("CreateAssunto")
        .WithOpenApi()
        .Produces<Assunto>(StatusCodes.Status201Created);

        group.MapPut("/{id:int}", async (int id, Assunto assunto, IAssuntoService service) =>
        {
            var updated = await service.UpdateAsync(id, assunto);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        })
        .WithName("UpdateAssunto")
        .WithOpenApi()
        .Produces<Assunto>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (int id, IAssuntoService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteAssunto")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
```

### Endpoints/MovimentacaoEndpoints.cs
```csharp
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
        .WithOpenApi()
        .Produces<IEnumerable<Movimentacao>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, IMovimentacaoService service) =>
        {
            var movimentacao = await service.GetByIdAsync(id);
            return movimentacao is not null ? Results.Ok(movimentacao) : Results.NotFound();
        })
        .WithName("GetMovimentacaoById")
        .WithOpenApi()
        .Produces<Movimentacao>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (Movimentacao movimentacao, IMovimentacaoService service) =>
        {
            var created = await service.CreateAsync(movimentacao);
            return Results.Created($"/api/movimentacoes/{created.Id}", created);
        })
        .WithName("CreateMovimentacao")
        .WithOpenApi()
        .Produces<Movimentacao>(StatusCodes.Status201Created);

        group.MapPut("/{id:int}", async (int id, Movimentacao movimentacao, IMovimentacaoService service) =>
        {
            var updated = await service.UpdateAsync(id, movimentacao);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        })
        .WithName("UpdateMovimentacao")
        .WithOpenApi()
        .Produces<Movimentacao>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (int id, IMovimentacaoService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteMovimentacao")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
```

### Endpoints/TipoDocumentoEndpoints.cs
```csharp
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
        .WithOpenApi()
        .Produces<IEnumerable<TipoDocumento>>(StatusCodes.Status200OK);

        group.MapGet("/{id:int}", async (int id, ITipoDocumentoService service) =>
        {
            var tipoDocumento = await service.GetByIdAsync(id);
            return tipoDocumento is not null ? Results.Ok(tipoDocumento) : Results.NotFound();
        })
        .WithName("GetTipoDocumentoById")
        .WithOpenApi()
        .Produces<TipoDocumento>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", async (TipoDocumento tipoDocumento, ITipoDocumentoService service) =>
        {
            var created = await service.CreateAsync(tipoDocumento);
            return Results.Created($"/api/tipos-documento/{created.Id}", created);
        })
        .WithName("CreateTipoDocumento")
        .WithOpenApi()
        .Produces<TipoDocumento>(StatusCodes.Status201Created);

        group.MapPut("/{id:int}", async (int id, TipoDocumento tipoDocumento, ITipoDocumentoService service) =>
        {
            var updated = await service.UpdateAsync(id, tipoDocumento);
            return updated is not null ? Results.Ok(updated) : Results.NotFound();
        })
        .WithName("UpdateTipoDocumento")
        .WithOpenApi()
        .Produces<TipoDocumento>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:int}", async (int id, ITipoDocumentoService service) =>
        {
            var deleted = await service.DeleteAsync(id);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteTipoDocumento")
        .WithOpenApi()
        .Produces(StatusCodes.Status204NoContent)
        .Produces(StatusCodes.Status404NotFound);
    }
}
```

---

## 📁 EXTENSIONS (2 arquivos)

### Extensions/ServiceCollectionExtensions.cs
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
        // Obter senha do User Secrets
        var defaultPassword = configuration["ConnectionStrings:DefaultPassword"] 
            ?? throw new InvalidOperationException("Database password not found in User Secrets. Run: dotnet user-secrets set \"ConnectionStrings:DefaultPassword\" \"your-password\"");

        // Construir connection string com senha do User Secrets
        var baseConnectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not found in configuration");
        
        var connectionString = $"{baseConnectionString};Password={defaultPassword}";

        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString));

        // Redis Cache
        var redisPassword = configuration["ConnectionStrings:RedisPassword"];
        var redisConnection = configuration.GetConnectionString("Redis");
        
        if (!string.IsNullOrEmpty(redisPassword))
        {
            redisConnection = $"{redisConnection},password={redisPassword}";
        }

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisConnection;
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
        var keycloakUrl = configuration["Keycloak:Url"];
        var realm = configuration["Keycloak:Realm"];
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
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

                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        var logger = context.HttpContext.RequestServices
                            .GetRequiredService<ILogger<Program>>();
                        logger.LogError(context.Exception, "Authentication failed");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        return services;
    }
}
```

### Extensions/WebApplicationExtensions.cs
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

## 📁 PRINCIPAL (1 arquivo)

### Program.cs
```csharp
using NetRedisASide2.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "NetRedisASide2 API",
        Version = "v1",
        Description = "API com .NET 9, PostgreSQL, Redis, Keycloak, Ollama e Weaviate"
    });

    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Bearer {token}\""
    });

    options.AddSecurityRequirement(new()
    {
        {
            new()
            {
                Reference = new()
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "NetRedisASide2 API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapApplicationEndpoints();

app.MapGet("/", () => Results.Redirect("/swagger"))
    .ExcludeFromDescription();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("HealthCheck")
    .WithTags("Health");

app.Run();
```

---

## 📊 RESUMO DO CÓDIGO

| Camada | Arquivos | Linhas | Características |
|--------|----------|--------|-----------------|
| **Models** | 3 | ~30 | Entidades simples e limpas |
| **Data** | 1 | ~50 | EF Core com configurações |
| **Repositories** | 6 | ~100 | Padrão Repository genérico |
| **Services** | 3 | ~450 | Cache Redis + Logging |
| **Endpoints** | 3 | ~150 | Minimal API + OpenAPI |
| **Extensions** | 2 | ~85 | DI + Configuração |
| **Principal** | 1 | ~60 | Program.cs limpo |
| **TOTAL** | **18** | **~925** | Código limpo e organizado |

## ✨ Características do Código

✅ **Clean Architecture** - Separação em camadas  
✅ **SOLID Principles** - Código manutenível  
✅ **Repository Pattern** - Abstração de dados  
✅ **Service Pattern** - Lógica de negócio  
✅ **Dependency Injection** - IoC  
✅ **Cache-Aside Pattern** - Performance  
✅ **Logging Estruturado** - Observabilidade  
✅ **Error Handling** - Robustez  
✅ **Async/Await** - Assíncrono  
✅ **OpenAPI/Swagger** - Documentação  

**🚀 Pronto para uso em produção!**
