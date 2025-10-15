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
├── Authentication/
│   ├── KeycloakAuthOptions.cs
│   └── AuthorizationPolicies.cs
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
    image: pgvector/pgvector:pg15
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

  keycloak:
    image: quay.io/keycloak/keycloak:23.0
    container_name: netredisaside2-keycloak
    environment:
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin123
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres-keycloak:5432/keycloak
      KC_DB_USERNAME: keycloak
      KC_DB_PASSWORD: keycloak123
    ports:
      - "8080:8080"
    command: start-dev
    depends_on:
      - postgres-keycloak

  postgres-keycloak:
    image: postgres:15
    container_name: netredisaside2-postgres-keycloak
    environment:
      POSTGRES_USER: keycloak
      POSTGRES_PASSWORD: keycloak123
      POSTGRES_DB: keycloak
    volumes:
      - postgres_keycloak_data:/var/lib/postgresql/data

volumes:
  postgres_data:
  postgres_keycloak_data:
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
  "Keycloak": {
    "Authority": "http://localhost:8080/realms/netredisaside2",
    "Audience": "netredisaside2-api",
    "RequireHttpsMetadata": false,
    "ValidateAudience": true,
    "ValidateIssuer": true
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
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Configurar PostgreSQL com pgvector
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        o => o.UseVector());
});

// Configurar Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "NetRedisASide2:";
});

// Configurar Autenticação JWT via Keycloak
var keycloakOptions = builder.Configuration.GetSection("Keycloak").Get<KeycloakAuthOptions>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = keycloakOptions!.Authority;
        options.Audience = keycloakOptions.Audience;
        options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = keycloakOptions.ValidateAudience,
            ValidateIssuer = keycloakOptions.ValidateIssuer,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudiences = new[] { keycloakOptions.Audience },
            ValidIssuer = keycloakOptions.Authority
        };
    });

// Configurar Autorização com Políticas
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.ConfigurePolicies(options);
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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "NetRedisASide2 API", 
        Version = "v1",
        Description = "API com autenticação Keycloak, PostgreSQL+pgvector e Redis"
    });

    // Configurar autenticação no Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

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

// IMPORTANTE: Ordem correta dos middlewares
app.UseAuthentication();
app.UseAuthorization();

// Mapear endpoints protegidos
app.MapAssuntoEndpoints();
app.MapMovimentacaoEndpoints();
app.MapTipoDocumentoEndpoints();
app.MapCacheEndpoints();

// Health check (público)
app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    application = "NetRedisASide2"
}))
.WithName("Health")
.WithTags("Health")
.AllowAnonymous();

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
using Pgvector.EntityFrameworkCore;

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

        // Habilitar extensão pgvector
        modelBuilder.HasPostgresExtension("vector");

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

        var assuntos = new[]
        {
            new Assunto { Nome = "Recursos Humanos", Descricao = "Documentos relacionados a RH" },
            new Assunto { Nome = "Financeiro", Descricao = "Documentos financeiros e contábeis" },
            new Assunto { Nome = "Jurídico", Descricao = "Documentos legais e contratos" },
            new Assunto { Nome = "Tecnologia", Descricao = "Documentos técnicos e especificações" }
        };
        context.Assuntos.AddRange(assuntos);
        context.SaveChanges();

        var movimentacoes = new[]
        {
            new Movimentacao { Nome = "Entrada", Descricao = "Documento recebido" },
            new Movimentacao { Nome = "Saída", Descricao = "Documento enviado" },
            new Movimentacao { Nome = "Arquivamento", Descricao = "Documento arquivado" },
            new Movimentacao { Nome = "Tramitação", Descricao = "Documento em trâmite" }
        };
        context.Movimentacoes.AddRange(movimentacoes);
        context.SaveChanges();

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
// Authentication/KeycloakAuthOptions.cs
// ============================================
public class KeycloakAuthOptions
{
    public string Authority { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public bool RequireHttpsMetadata { get; set; }
    public bool ValidateAudience { get; set; }
    public bool ValidateIssuer { get; set; }
}

// ============================================
// Authentication/AuthorizationPolicies.cs
// ============================================
using Microsoft.AspNetCore.Authorization;

public static class AuthorizationPolicies
{
    public const string AdminPolicy = "AdminOnly";
    public const string UserPolicy = "UserAccess";
    public const string ManagerPolicy = "ManagerAccess";

    public static void ConfigurePolicies(AuthorizationOptions options)
    {
        // Política para Administradores
        options.AddPolicy(AdminPolicy, policy =>
            policy.RequireRole("admin"));

        // Política para Usuários (admin ou user)
        options.AddPolicy(UserPolicy, policy =>
            policy.RequireRole("admin", "user"));

        // Política para Gerentes (admin ou manager)
        options.AddPolicy(ManagerPolicy, policy =>
            policy.RequireRole("admin", "manager"));
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
// Services (IAssuntoService, AssuntoService, etc.)
// Código idêntico ao anterior
// ============================================

// ============================================
// Endpoints/AssuntoEndpoints.cs
// ============================================
using Microsoft.AspNetCore.Authorization;

public static class AssuntoEndpoints
{
    public static void MapAssuntoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assuntos")
            .WithTags("Assuntos")
            .RequireAuthorization(AuthorizationPolicies.UserPolicy);

        // GET - Listar (requer autenticação de usuário)
        group.MapGet("/", async (IAssuntoService service) =>
        {
            var result = await service.GetAllAsync();
            return Results.Ok(result);
        })
        .WithName("GetAllAssuntos");

        // GET por ID (requer autenticação de usuário)
        group.MapGet("/{id}", async (int id, IAssuntoService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("GetAssunto");

        // POST - Criar (requer role manager ou admin)
        group.MapPost("/", async (Assunto assunto, IAssuntoService service) =>
        {
            var result = await service.CreateAsync(assunto);
            return Results.Created($"/api/assuntos/{result.Data!.Id}", result);
        })
        .WithName("CreateAssunto")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        // PUT - Atualizar (requer role manager ou admin)
        group.MapPut("/{id}", async (int id, Assunto assunto, IAssuntoService service) =>
        {
            var result = await service.UpdateAsync(id, assunto);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateAssunto")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        // DELETE - Deletar (requer role admin)
        group.MapDelete("/{id}", async (int id, IAssuntoService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Success ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteAssunto")
        .RequireAuthorization(AuthorizationPolicies.AdminPolicy);
    }
}

// ============================================
// Endpoints/MovimentacaoEndpoints.cs
// ============================================
using Microsoft.AspNetCore.Authorization;

public static class MovimentacaoEndpoints
{
    public static void MapMovimentacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/movimentacoes")
            .WithTags("Movimentações")
            .RequireAuthorization(AuthorizationPolicies.UserPolicy);

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
        .WithName("CreateMovimentacao")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        group.MapPut("/{id}", async (int id, Movimentacao movimentacao, IMovimentacaoService service) =>
        {
            var result = await service.UpdateAsync(id, movimentacao);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateMovimentacao")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        group.MapDelete("/{id}", async (int id, IMovimentacaoService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Success ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteMovimentacao")
        .RequireAuthorization(AuthorizationPolicies.AdminPolicy);
    }
}

// ============================================
// Endpoints/TipoDocumentoEndpoints.cs
// ============================================
using Microsoft.AspNetCore.Authorization;

public static class TipoDocumentoEndpoints
{
    public static void MapTipoDocumentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tipos-documento")
            .WithTags("Tipos de Documento")
            .RequireAuthorization(AuthorizationPolicies.UserPolicy);

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
        .WithName("CreateTipoDocumento")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        group.MapPut("/{id}", async (int id, TipoDocumento tipoDocumento, ITipoDocumentoService service) =>
        {
            var result = await service.UpdateAsync(id, tipoDocumento);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateTipoDocumento")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        group.MapDelete("/{id}", async (int id, ITipoDocumentoService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Success ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteTipoDocumento")
        .RequireAuthorization(AuthorizationPolicies.AdminPolicy);
    }
}

// ============================================
// Endpoints/CacheEndpoints.cs
// ============================================
using Microsoft.AspNetCore.Authorization;

public static class CacheEndpoints
{
    public static void MapCacheEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cache")
            .WithTags("Cache")
            .RequireAuthorization(AuthorizationPolicies.AdminPolicy);

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
                security = "Keycloak JWT Authentication",
                database = "PostgreSQL + pgvector",
                cache = "Redis (IDistributedCache)"
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
    <PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
    <PackageReference Include="Pgvector.EntityFrameworkCore" Version="0.2.0" />
    <PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.0.0" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
</Project>
*/

// ============================================
// GUIA DE CONFIGURAÇÃO DO KEYCLOAK
// ============================================
/*
1. Acessar Keycloak: http://localhost:8080
   - Usuário: admin
   - Senha: admin123

2. Criar Realm "netredisaside2"
   - Realm Settings → General → Realm ID: netredisaside2

3. Criar Client "netredisaside2-api"
   - Client ID: netredisaside2-api
   - Client Protocol: openid-connect
   - Access Type: bearer-only ou public
   - Valid Redirect URIs: *

4. Criar Roles (Realm Roles)
   - admin
   - manager
   - user

5. Criar Usuários
   Usuário Admin:
   - Username: admin
   - Email: admin@netredis.com
   - Password: admin123
   - Role Mappings: admin

   Usuário Manager:
   - Username: manager
   - Email: manager@netredis.com
   - Password: manager123
   - Role Mappings: manager

   Usuário Normal:
   - Username: user
   - Email: user@netredis.com
   - Password: user123
   - Role Mappings: user

6. Obter Token
curl -X POST 'http://localhost:8080/realms/netredisaside2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'username=admin' \
  -d 'password=admin123' \
  -d 'grant_type=password'

7. Usar Token nas Requisições
curl -H "Authorization: Bearer {ACCESS_TOKEN}" \
  http://localhost:5000/api/assuntos
*/