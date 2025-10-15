// ============================================
// ESTRUTURA DO PROJETO NetRedisASide2
// ============================================
/*
NetRedisASide2/
├── Program.cs
├── appsettings.json
├── docker-compose.yml
├── Models/
│   ├── Assunto.cs
│   ├── Movimentacao.cs
│   ├── TipoDocumento.cs
│   └── ServiceResult.cs
├── Data/
│   ├── AppDbContext.cs
│   └── DbInitializer.cs
├── Repositories/
│   ├── IAssuntoRepository.cs
│   ├── AssuntoRepository.cs
│   ├── IMovimentacaoRepository.cs
│   ├── MovimentacaoRepository.cs
│   ├── ITipoDocumentoRepository.cs
│   └── TipoDocumentoRepository.cs
├── Services/
│   ├── IAssuntoService.cs
│   ├── AssuntoService.cs
│   ├── IMovimentacaoService.cs
│   ├── MovimentacaoService.cs
│   ├── ITipoDocumentoService.cs
│   └── TipoDocumentoService.cs
├── Helpers/
│   ├── ICacheHelper.cs
│   └── CacheHelper.cs
└── Endpoints/
    ├── AssuntoEndpoints.cs
    ├── MovimentacaoEndpoints.cs
    ├── TipoDocumentoEndpoints.cs
    └── CacheEndpoints.cs
*/

// ============================================
// docker-compose.yml
// ============================================
/*
version: '3.8'

services:
  postgres:
    image: postgres:15
    container_name: netredisaside2-postgres
    environment:
      POSTGRES_USER: admin
      POSTGRES_PASSWORD: admin123
      POSTGRES_DB: netredisaside2
    ports:
      - "5432:5432"
    volumes:
      - postgres_data:/var/lib/postgresql/data

  redis:
    image: redis:7
    container_name: netredisaside2-redis
    ports:
      - "6379:6379"

volumes:
  postgres_data:
*/

// ============================================
// appsettings.json
// ============================================
/*
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=netredisaside2;Username=admin;Password=admin123",
    "Redis": "localhost:6379"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  },
  "AllowedHosts": "*"
}
*/

// ============================================
// Program.cs
// ============================================
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configurar PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "NetRedisASide2:";
});

// Registrar CacheHelper
builder.Services.AddScoped<ICacheHelper, CacheHelper>();

// Registrar Repositories
builder.Services.AddScoped<IAssuntoRepository, AssuntoRepository>();
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
builder.Services.AddScoped<ITipoDocumentoRepository, TipoDocumentoRepository>();

// Registrar Services
builder.Services.AddScoped<IAssuntoService, AssuntoService>();
builder.Services.AddScoped<IMovimentacaoService, MovimentacaoService>();
builder.Services.AddScoped<ITipoDocumentoService, TipoDocumentoService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        context.Database.Migrate();
        DbInitializer.Initialize(context);
        logger.LogInformation("✅ Database initialized successfully");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error initializing database");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mapear endpoints (classes separadas)
app.MapAssuntoEndpoints();
app.MapMovimentacaoEndpoints();
app.MapTipoDocumentoEndpoints();
app.MapCacheEndpoints();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    application = "NetRedisASide2"
}))
.WithName("Health")
.WithTags("Health");

app.Run();

// ============================================
// Models/Assunto.cs
// ============================================
using System.ComponentModel.DataAnnotations;

public class Assunto
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}

// ============================================
// Models/Movimentacao.cs
// ============================================
using System.ComponentModel.DataAnnotations;

public class Movimentacao
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}

// ============================================
// Models/TipoDocumento.cs
// ============================================
using System.ComponentModel.DataAnnotations;

public class TipoDocumento
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [MaxLength(200)]
    public string Nome { get; set; } = string.Empty;
    
    [MaxLength(1000)]
    public string Descricao { get; set; } = string.Empty;
    
    public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    public DateTime DataAtualizacao { get; set; } = DateTime.UtcNow;
}

// ============================================
// Models/ServiceResult.cs
// ============================================
public class ServiceResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string Source { get; set; } = "DATABASE";
    public bool CacheInvalidated { get; set; }
}

// ============================================
// Data/AppDbContext.cs
// ============================================
using Microsoft.EntityFrameworkCore;

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

        // Índices
        modelBuilder.Entity<Assunto>()
            .HasIndex(a => a.Nome);

        modelBuilder.Entity<Movimentacao>()
            .HasIndex(m => m.Nome);

        modelBuilder.Entity<TipoDocumento>()
            .HasIndex(t => t.Nome);
    }
}

// ============================================
// Data/DbInitializer.cs
// ============================================
public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        if (context.Assuntos.Any())
            return;

        // Seed Assuntos
        var assuntos = new[]
        {
            new Assunto { Nome = "Recursos Humanos", Descricao = "Documentos relacionados a RH" },
            new Assunto { Nome = "Financeiro", Descricao = "Documentos financeiros e contábeis" },
            new Assunto { Nome = "Jurídico", Descricao = "Documentos legais e contratos" },
            new Assunto { Nome = "Tecnologia", Descricao = "Documentos técnicos e especificações" }
        };
        context.Assuntos.AddRange(assuntos);
        context.SaveChanges();

        // Seed Movimentações
        var movimentacoes = new[]
        {
            new Movimentacao { Nome = "Entrada", Descricao = "Documento recebido" },
            new Movimentacao { Nome = "Saída", Descricao = "Documento enviado" },
            new Movimentacao { Nome = "Arquivamento", Descricao = "Documento arquivado" },
            new Movimentacao { Nome = "Tramitação", Descricao = "Documento em trâmite" }
        };
        context.Movimentacoes.AddRange(movimentacoes);
        context.SaveChanges();

        // Seed Tipos de Documento
        var tiposDocumento = new[]
        {
            new TipoDocumento { Nome = "Ofício", Descricao = "Correspondência oficial" },
            new TipoDocumento { Nome = "Memorando", Descricao = "Comunicação interna" },
            new TipoDocumento { Nome = "Ata", Descricao = "Registro de reunião" },
            new TipoDocumento { Nome = "Relatório", Descricao = "Documento técnico descritivo" },
            new TipoDocumento { Nome = "Contrato", Descricao = "Acordo formal entre partes" }
        };
        context.TiposDocumento.AddRange(tiposDocumento);
        context.SaveChanges();
    }
}

// ============================================
// Helpers/ICacheHelper.cs
// ============================================
public interface ICacheHelper
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, int minutes = 10);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
    Task ClearAllAsync();
}

// ============================================
// Helpers/CacheHelper.cs
// ============================================
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class CacheHelper : ICacheHelper
{
    private readonly IDistributedCache _cache;
    private readonly ILogger<CacheHelper> _logger;
    private readonly List<string> _knownKeys = new();

    public CacheHelper(IDistributedCache cache, ILogger<CacheHelper> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var bytes = await _cache.GetAsync(key);
            
            if (bytes == null || bytes.Length == 0)
            {
                _logger.LogInformation("❌ CACHE MISS: {Key}", key);
                return default;
            }

            _logger.LogInformation("✅ CACHE HIT: {Key}", key);
            var json = System.Text.Encoding.UTF8.GetString(bytes);
            return JsonSerializer.Deserialize<T>(json);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar cache: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, int minutes = 10)
    {
        try
        {
            var json = JsonSerializer.Serialize(value);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes)
            };
            
            await _cache.SetAsync(key, bytes, options);
            
            if (!_knownKeys.Contains(key))
                _knownKeys.Add(key);
            
            _logger.LogInformation("💾 CACHE SET: {Key} ({Minutes}min)", key, minutes);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao salvar cache: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _cache.RemoveAsync(key);
            _knownKeys.Remove(key);
            _logger.LogInformation("🗑️ CACHE REMOVED: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover cache: {Key}", key);
        }
    }

    public async Task RemoveByPatternAsync(string pattern)
    {
        var keysToRemove = _knownKeys.Where(k => k.Contains(pattern)).ToList();
        
        foreach (var key in keysToRemove)
        {
            await RemoveAsync(key);
        }
    }

    public async Task ClearAllAsync()
    {
        foreach (var key in _knownKeys.ToList())
        {
            await RemoveAsync(key);
        }
    }
}

// ============================================
// Repositories/IAssuntoRepository.cs
// ============================================
public interface IAssuntoRepository
{
    Task<List<Assunto>> GetAllAsync();
    Task<Assunto?> GetByIdAsync(int id);
    Task<Assunto> CreateAsync(Assunto assunto);
    Task<Assunto?> UpdateAsync(int id, Assunto assunto);
    Task<bool> DeleteAsync(int id);
}

// ============================================
// Repositories/AssuntoRepository.cs
// ============================================
using Microsoft.EntityFrameworkCore;

public class AssuntoRepository : IAssuntoRepository
{
    private readonly AppDbContext _context;

    public AssuntoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Assunto>> GetAllAsync()
    {
        return await _context.Assuntos
            .AsNoTracking()
            .OrderBy(a => a.Nome)
            .ToListAsync();
    }

    public async Task<Assunto?> GetByIdAsync(int id)
    {
        return await _context.Assuntos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assunto> CreateAsync(Assunto assunto)
    {
        assunto.DataCriacao = DateTime.UtcNow;
        assunto.DataAtualizacao = DateTime.UtcNow;
        
        _context.Assuntos.Add(assunto);
        await _context.SaveChangesAsync();
        return assunto;
    }

    public async Task<Assunto?> UpdateAsync(int id, Assunto assunto)
    {
        var existing = await _context.Assuntos.FindAsync(id);
        
        if (existing == null)
            return null;

        existing.Nome = assunto.Nome;
        existing.Descricao = assunto.Descricao;
        existing.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var assunto = await _context.Assuntos.FindAsync(id);
        
        if (assunto == null)
            return false;

        _context.Assuntos.Remove(assunto);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ============================================
// Repositories/IMovimentacaoRepository.cs
// ============================================
public interface IMovimentacaoRepository
{
    Task<List<Movimentacao>> GetAllAsync();
    Task<Movimentacao?> GetByIdAsync(int id);
    Task<Movimentacao> CreateAsync(Movimentacao movimentacao);
    Task<Movimentacao?> UpdateAsync(int id, Movimentacao movimentacao);
    Task<bool> DeleteAsync(int id);
}

// ============================================
// Repositories/MovimentacaoRepository.cs
// ============================================
using Microsoft.EntityFrameworkCore;

public class MovimentacaoRepository : IMovimentacaoRepository
{
    private readonly AppDbContext _context;

    public MovimentacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Movimentacao>> GetAllAsync()
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .OrderBy(m => m.Nome)
            .ToListAsync();
    }

    public async Task<Movimentacao?> GetByIdAsync(int id)
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Movimentacao> CreateAsync(Movimentacao movimentacao)
    {
        movimentacao.DataCriacao = DateTime.UtcNow;
        movimentacao.DataAtualizacao = DateTime.UtcNow;
        
        _context.Movimentacoes.Add(movimentacao);
        await _context.SaveChangesAsync();
        return movimentacao;
    }

    public async Task<Movimentacao?> UpdateAsync(int id, Movimentacao movimentacao)
    {
        var existing = await _context.Movimentacoes.FindAsync(id);
        
        if (existing == null)
            return null;

        existing.Nome = movimentacao.Nome;
        existing.Descricao = movimentacao.Descricao;
        existing.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var movimentacao = await _context.Movimentacoes.FindAsync(id);
        
        if (movimentacao == null)
            return false;

        _context.Movimentacoes.Remove(movimentacao);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ============================================
// Repositories/ITipoDocumentoRepository.cs
// ============================================
public interface ITipoDocumentoRepository
{
    Task<List<TipoDocumento>> GetAllAsync();
    Task<TipoDocumento?> GetByIdAsync(int id);
    Task<TipoDocumento> CreateAsync(TipoDocumento tipoDocumento);
    Task<TipoDocumento?> UpdateAsync(int id, TipoDocumento tipoDocumento);
    Task<bool> DeleteAsync(int id);
}

// ============================================
// Repositories/TipoDocumentoRepository.cs
// ============================================
using Microsoft.EntityFrameworkCore;

public class TipoDocumentoRepository : ITipoDocumentoRepository
{
    private readonly AppDbContext _context;

    public TipoDocumentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TipoDocumento>> GetAllAsync()
    {
        return await _context.TiposDocumento
            .AsNoTracking()
            .OrderBy(t => t.Nome)
            .ToListAsync();
    }

    public async Task<TipoDocumento?> GetByIdAsync(int id)
    {
        return await _context.TiposDocumento
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TipoDocumento> CreateAsync(TipoDocumento tipoDocumento)
    {
        tipoDocumento.DataCriacao = DateTime.UtcNow;
        tipoDocumento.DataAtualizacao = DateTime.UtcNow;
        
        _context.TiposDocumento.Add(tipoDocumento);
        await _context.SaveChangesAsync();
        return tipoDocumento;
    }

    public async Task<TipoDocumento?> UpdateAsync(int id, TipoDocumento tipoDocumento)
    {
        var existing = await _context.TiposDocumento.FindAsync(id);
        
        if (existing == null)
            return null;

        existing.Nome = tipoDocumento.Nome;
        existing.Descricao = tipoDocumento.Descricao;
        existing.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var tipoDocumento = await _context.TiposDocumento.FindAsync(id);
        
        if (tipoDocumento == null)
            return false;

        _context.TiposDocumento.Remove(tipoDocumento);
        await _context.SaveChangesAsync();
        return true;
    }
}

// ============================================
// Services/IAssuntoService.cs
// ============================================
public interface IAssuntoService
{
    Task<ServiceResult<List<Assunto>>> GetAllAsync();
    Task<ServiceResult<Assunto>> GetByIdAsync(int id);
    Task<ServiceResult<Assunto>> CreateAsync(Assunto assunto);
    Task<ServiceResult<Assunto>> UpdateAsync(int id, Assunto assunto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}

// ============================================
// Services/AssuntoService.cs
// ============================================
public class AssuntoService : IAssuntoService
{
    private readonly IAssuntoRepository _repository;
    private readonly ICacheHelper _cache;
    private const string CACHE_KEY_ALL = "assuntos:all";
    private const string CACHE_KEY_PREFIX = "assunto:";

    public AssuntoService(IAssuntoRepository repository, ICacheHelper cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ServiceResult<List<Assunto>>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<Assunto>>(CACHE_KEY_ALL);
        if (cached != null)
        {
            return new ServiceResult<List<Assunto>>
            {
                Success = true,
                Data = cached,
                Source = "CACHE",
                Message = "Dados do cache Redis"
            };
        }

        var assuntos = await _repository.GetAllAsync();
        await _cache.SetAsync(CACHE_KEY_ALL, assuntos, 10);

        return new ServiceResult<List<Assunto>>
        {
            Success = true,
            Data = assuntos,
            Source = "DATABASE",
            Message = "Dados do PostgreSQL (cacheados por 10min)"
        };
    }

    public async Task<ServiceResult<Assunto>> GetByIdAsync(int id)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}{id}";

        var cached = await _cache.GetAsync<Assunto>(cacheKey);
        if (cached != null)
        {
            return new ServiceResult<Assunto>
            {
                Success = true,
                Data = cached,
                Source = "CACHE"
            };
        }

        var assunto = await _repository.GetByIdAsync(id);
        
        if (assunto == null)
        {
            return new ServiceResult<Assunto>
            {
                Success = false,
                Message = "Assunto não encontrado"
            };
        }

        await _cache.SetAsync(cacheKey, assunto, 15);

        return new ServiceResult<Assunto>
        {
            Success = true,
            Data = assunto,
            Source = "DATABASE"
        };
    }

    public async Task<ServiceResult<Assunto>> CreateAsync(Assunto assunto)
    {
        var created = await _repository.CreateAsync(assunto);
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<Assunto>
        {
            Success = true,
            Data = created,
            Message = "Assunto criado com sucesso",
            CacheInvalidated = true
        };
    }

    public async Task<ServiceResult<Assunto>> UpdateAsync(int id, Assunto assunto)
    {
        var updated = await _repository.UpdateAsync(id, assunto);
        
        if (updated == null)
        {
            return new ServiceResult<Assunto>
            {
                Success = false,
                Message = "Assunto não encontrado"
            };
        }

        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<Assunto>
        {
            Success = true,
            Data = updated,
            Message = "Assunto atualizado com sucesso",
            CacheInvalidated = true
        };
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        
        if (!deleted)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "Assunto não encontrado"
            };
        }

        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<bool>
        {
            Success = true,
            Data = deleted,
            Message = "Assunto deletado com sucesso",
            CacheInvalidated = true
        };
    }
}

// ============================================
// Services/IMovimentacaoService.cs
// ============================================
public interface IMovimentacaoService
{
    Task<ServiceResult<List<Movimentacao>>> GetAllAsync();
    Task<ServiceResult<Movimentacao>> GetByIdAsync(int id);
    Task<ServiceResult<Movimentacao>> CreateAsync(Movimentacao movimentacao);
    Task<ServiceResult<Movimentacao>> UpdateAsync(int id, Movimentacao movimentacao);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}

// ============================================
// Services/MovimentacaoService.cs
// ============================================
public class MovimentacaoService : IMovimentacaoService
{
    private readonly IMovimentacaoRepository _repository;
    private readonly ICacheHelper _cache;
    private const string CACHE_KEY_ALL = "movimentacoes:all";
    private const string CACHE_KEY_PREFIX = "movimentacao:";

    public MovimentacaoService(IMovimentacaoRepository repository, ICacheHelper cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ServiceResult<List<Movimentacao>>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<Movimentacao>>(CACHE_KEY_ALL);
        if (cached != null)
        {
            return new ServiceResult<List<Movimentacao>>
            {
                Success = true,
                Data = cached,
                Source = "CACHE"
            };
        }

        var movimentacoes = await _repository.GetAllAsync();
        await _cache.SetAsync(CACHE_KEY_ALL, movimentacoes, 10);

        return new ServiceResult<List<Movimentacao>>
        {
            Success = true,
            Data = movimentacoes,
            Source = "DATABASE"
        };
    }

    public async Task<ServiceResult<Movimentacao>> GetByIdAsync(int id)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}{id}";

        var cached = await _cache.GetAsync<Movimentacao>(cacheKey);
        if (cached != null)
        {
            return new ServiceResult<Movimentacao>
            {
                Success = true,
                Data = cached,
                Source = "CACHE"
            };
        }

        var movimentacao = await _repository.GetByIdAsync(id);
        
        if (movimentacao == null)
        {
            return new ServiceResult<Movimentacao>
            {
                Success = false,
                Message = "Movimentação não encontrada"
            };
        }

        await _cache.SetAsync(cacheKey, movimentacao, 15);

        return new ServiceResult<Movimentacao>
        {
            Success = true,
            Data = movimentacao,
            Source = "DATABASE"
        };
    }

    public async Task<ServiceResult<Movimentacao>> CreateAsync(Movimentacao movimentacao)
    {
        var created = await _repository.CreateAsync(movimentacao);
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<Movimentacao>
        {
            Success = true,
            Data = created,
            Message = "Movimentação criada com sucesso",
            CacheInvalidated = true
        };
    }

    public async Task<ServiceResult<Movimentacao>> UpdateAsync(int id, Movimentacao movimentacao)
    {
        var updated = await _repository.UpdateAsync(id, movimentacao);
        
        if (updated == null)
        {
            return new ServiceResult<Movimentacao>
            {
                Success = false,
                Message = "Movimentação não encontrada"
            };
        }

        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<Movimentacao>
        {
            Success = true,
            Data = updated,
            Message = "Movimentação atualizada com sucesso",
            CacheInvalidated = true
        };
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        
        if (!deleted)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "Movimentação não encontrada"
            };
        }

        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<bool>
        {
            Success = true,
            Data = deleted,
            Message = "Movimentação deletada com sucesso",
            CacheInvalidated = true
        };
    }
}

// ============================================
// Services/ITipoDocumentoService.cs
// ============================================
public interface ITipoDocumentoService
{
    Task<ServiceResult<List<TipoDocumento>>> GetAllAsync();
    Task<ServiceResult<TipoDocumento>> GetByIdAsync(int id);
    Task<ServiceResult<TipoDocumento>> CreateAsync(TipoDocumento tipoDocumento);
    Task<ServiceResult<TipoDocumento>> UpdateAsync(int id, TipoDocumento tipoDocumento);
    Task<ServiceResult<bool>> DeleteAsync(int id);
}

// ============================================
// Services/TipoDocumentoService.cs
// ============================================
public class TipoDocumentoService : ITipoDocumentoService
{
    private readonly ITipoDocumentoRepository _repository;
    private readonly ICacheHelper _cache;
    private const string CACHE_KEY_ALL = "tiposdocumento:all";
    private const string CACHE_KEY_PREFIX = "tipodocumento:";

    public TipoDocumentoService(ITipoDocumentoRepository repository, ICacheHelper cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ServiceResult<List<TipoDocumento>>> GetAllAsync()
    {
        var cached = await _cache.GetAsync<List<TipoDocumento>>(CACHE_KEY_ALL);
        if (cached != null)
        {
            return new ServiceResult<List<TipoDocumento>>
            {
                Success = true,
                Data = cached,
                Source = "CACHE"
            };
        }

        var tipos = await _repository.GetAllAsync();
        await _cache.SetAsync(CACHE_KEY_ALL, tipos, 10);

        return new ServiceResult<List<TipoDocumento>>
        {
            Success = true,
            Data = tipos,
            Source = "DATABASE"
        };
    }

    public async Task<ServiceResult<TipoDocumento>> GetByIdAsync(int id)
    {
        var cacheKey = $"{CACHE_KEY_PREFIX}{id}";

        var cached = await _cache.GetAsync<TipoDocumento>(cacheKey);
        if (cached != null)
        {
            return new ServiceResult<TipoDocumento>
            {
                Success = true,
                Data = cached,
                Source = "CACHE"
            };
        }

        var tipo = await _repository.GetByIdAsync(id);
        
        if (tipo == null)
        {
            return new ServiceResult<TipoDocumento>
            {
                Success = false,
                Message = "Tipo de documento não encontrado"
            };
        }

        await _cache.SetAsync(cacheKey, tipo, 15);

        return new ServiceResult<TipoDocumento>
        {
            Success = true,
            Data = tipo,
            Source = "DATABASE"
        };
    }

    public async Task<ServiceResult<TipoDocumento>> CreateAsync(TipoDocumento tipoDocumento)
    {
        var created = await _repository.CreateAsync(tipoDocumento);
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<TipoDocumento>
        {
            Success = true,
            Data = created,
            Message = "Tipo de documento criado com sucesso",
            CacheInvalidated = true
        };
    }

    public async Task<ServiceResult<TipoDocumento>> UpdateAsync(int id, TipoDocumento tipoDocumento)
    {
        var updated = await _repository.UpdateAsync(id, tipoDocumento);
        
        if (updated == null)
        {
            return new ServiceResult<TipoDocumento>
            {
                Success = false,
                Message = "Tipo de documento não encontrado"
            };
        }

        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<TipoDocumento>
        {
            Success = true,
            Data = updated,
            Message = "Tipo de documento atualizado com sucesso",
            CacheInvalidated = true
        };
    }

    public async Task<ServiceResult<bool>> DeleteAsync(int id)
    {
        var deleted = await _repository.DeleteAsync(id);
        
        if (!deleted)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                Message = "Tipo de documento não encontrado"
            };
        }

        await _cache.RemoveAsync($"{CACHE_KEY_PREFIX}{id}");
        await _cache.RemoveAsync(CACHE_KEY_ALL);

        return new ServiceResult<bool>
        {
            Success = true,
            Data = deleted,
            Message = "Tipo de documento deletado com sucesso",
            CacheInvalidated = true
        };
    }
}

// ============================================
// Endpoints/AssuntoEndpoints.cs
// ============================================
public static class AssuntoEndpoints
{
    public static void MapAssuntoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assuntos")
            .WithTags("Assuntos");

        group.MapGet("/", async (IAssuntoService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllAssuntos");

        group.MapGet("/{id}", async (int id, IAssuntoService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetAssunto");

        group.MapPost("/", async (Assunto assunto, IAssuntoService service) =>
        {
            var result = await service.CreateAsync(assunto);
            return Results.Created($"/api/assuntos/{result.Data!.Id}", result);
        })
        .WithName("CreateAssunto");

        group.MapPut("/{id}", async (int id, Assunto assunto, IAssuntoService service) =>
        {
            var result = await service.UpdateAsync(id, assunto);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateAssunto");

        group.MapDelete("/{id}", async (int id, IAssuntoService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Success ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteAssunto");
    }
}

// ============================================
// Endpoints/MovimentacaoEndpoints.cs
// ============================================
public static class MovimentacaoEndpoints
{
    public static void MapMovimentacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/movimentacoes")
            .WithTags("Movimentações");

        group.MapGet("/", async (IMovimentacaoService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllMovimentacoes");

        group.MapGet("/{id}", async (int id, IMovimentacaoService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetMovimentacao");

        group.MapPost("/", async (Movimentacao movimentacao, IMovimentacaoService service) =>
        {
            var result = await service.CreateAsync(movimentacao);
            return Results.Created($"/api/movimentacoes/{result.Data!.Id}", result);
        })
        .WithName("CreateMovimentacao");

        group.MapPut("/{id}", async (int id, Movimentacao movimentacao, IMovimentacaoService service) =>
        {
            var result = await service.UpdateAsync(id, movimentacao);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateMovimentacao");

        group.MapDelete("/{id}", async (int id, IMovimentacaoService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Success ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteMovimentacao");
    }
}

// ============================================
// Endpoints/TipoDocumentoEndpoints.cs
// ============================================
public static class TipoDocumentoEndpoints
{
    public static void MapTipoDocumentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tipos-documento")
            .WithTags("Tipos de Documento");

        group.MapGet("/", async (ITipoDocumentoService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllTiposDocumento");

        group.MapGet("/{id}", async (int id, ITipoDocumentoService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetTipoDocumento");

        group.MapPost("/", async (TipoDocumento tipoDocumento, ITipoDocumentoService service) =>
        {
            var result = await service.CreateAsync(tipoDocumento);
            return Results.Created($"/api/tipos-documento/{result.Data!.Id}", result);
        })
        .WithName("CreateTipoDocumento");

        group.MapPut("/{id}", async (int id, TipoDocumento tipoDocumento, ITipoDocumentoService service) =>
        {
            var result = await service.UpdateAsync(id, tipoDocumento);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateTipoDocumento");

        group.MapDelete("/{id}", async (int id, ITipoDocumentoService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Success ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteTipoDocumento");
    }
}

// ============================================
// Endpoints/CacheEndpoints.cs
// ============================================
public static class CacheEndpoints
{
    public static void MapCacheEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cache")
            .WithTags("Cache");

        group.MapDelete("/clear", async (ICacheHelper cache) =>
        {
            await cache.ClearAllAsync();
            return Results.Ok(new { message = "Cache limpo com sucesso" });
        })
        .WithName("ClearCache");

        group.MapGet("/info", () =>
        {
            return Results.Ok(new
            {
                application = "NetRedisASide2",
                description = "Sistema de Gestão Documental com Cache",
                architecture = new
                {
                    database = "PostgreSQL",
                    cache = "Redis (IDistributedCache)",
                    orm = "Entity Framework Core",
                    pattern = "Repository + Service + Cache Aside",
                    endpoints = "Classes Separadas"
                },
                entities = new[] { "Assuntos", "Movimentações", "Tipos de Documento" }
            });
        })
        .WithName("GetCacheInfo");
    }
}

// ============================================
// .csproj
// ============================================
/*
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
</Project>
*/