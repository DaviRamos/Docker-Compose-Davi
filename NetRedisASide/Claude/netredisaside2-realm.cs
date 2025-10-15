// ============================================
// ESTRUTURA DO PROJETO NetRedisASide2
// ============================================
/*
NetRedisASide2/
├── Program.cs
├── appsettings.json
├── docker-compose.yml
├── keycloak/
│   └── realm-netredisaside2.json      # ✅ Realm pré-configurado
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
// keycloak/realm-netredisaside2.json
// ============================================
/*
{
  "realm": "netredisaside2",
  "enabled": true,
  "sslRequired": "none",
  "registrationAllowed": false,
  "loginWithEmailAllowed": true,
  "duplicateEmailsAllowed": false,
  "resetPasswordAllowed": true,
  "editUsernameAllowed": false,
  "bruteForceProtected": true,
  "permanentLockout": false,
  "maxFailureWaitSeconds": 900,
  "minimumQuickLoginWaitSeconds": 60,
  "waitIncrementSeconds": 60,
  "quickLoginCheckMilliSeconds": 1000,
  "maxDeltaTimeSeconds": 43200,
  "failureFactor": 5,
  "roles": {
    "realm": [
      {
        "name": "admin",
        "description": "Administrador com acesso total",
        "composite": false,
        "clientRole": false
      },
      {
        "name": "manager",
        "description": "Gerente com permissões de criar e editar",
        "composite": false,
        "clientRole": false
      },
      {
        "name": "user",
        "description": "Usuário com permissões de leitura",
        "composite": false,
        "clientRole": false
      }
    ]
  },
  "clients": [
    {
      "clientId": "netredisaside2-api",
      "name": "NetRedisASide2 API",
      "description": "API Backend",
      "enabled": true,
      "clientAuthenticatorType": "client-secret",
      "secret": "netredisaside2-secret",
      "redirectUris": ["*"],
      "webOrigins": ["*"],
      "publicClient": false,
      "protocol": "openid-connect",
      "standardFlowEnabled": true,
      "implicitFlowEnabled": false,
      "directAccessGrantsEnabled": true,
      "serviceAccountsEnabled": false,
      "attributes": {
        "access.token.lifespan": "1800"
      },
      "fullScopeAllowed": true,
      "defaultClientScopes": [
        "web-origins",
        "profile",
        "roles",
        "email"
      ]
    }
  ],
  "users": [
    {
      "username": "admin",
      "enabled": true,
      "email": "admin@netredisaside2.com",
      "emailVerified": true,
      "firstName": "Admin",
      "lastName": "Sistema",
      "credentials": [
        {
          "type": "password",
          "value": "admin123",
          "temporary": false
        }
      ],
      "realmRoles": ["admin"],
      "clientRoles": {}
    },
    {
      "username": "manager",
      "enabled": true,
      "email": "manager@netredisaside2.com",
      "emailVerified": true,
      "firstName": "Manager",
      "lastName": "Sistema",
      "credentials": [
        {
          "type": "password",
          "value": "manager123",
          "temporary": false
        }
      ],
      "realmRoles": ["manager"],
      "clientRoles": {}
    },
    {
      "username": "user",
      "enabled": true,
      "email": "user@netredisaside2.com",
      "emailVerified": true,
      "firstName": "User",
      "lastName": "Sistema",
      "credentials": [
        {
          "type": "password",
          "value": "user123",
          "temporary": false
        }
      ],
      "realmRoles": ["user"],
      "clientRoles": {}
    }
  ]
}
*/

// ============================================
// docker-compose.yml
// ============================================
/*
version: '3.8'

services:
  # PostgreSQL para aplicação
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
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U admin -d netredisaside2"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Redis para cache
  redis:
    image: redis:7-alpine
    container_name: netredisaside2-redis
    ports:
      - "6379:6379"
    volumes:
      - redis_data:/data
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD", "redis-cli", "ping"]
      interval: 10s
      timeout: 5s
      retries: 5

  # PostgreSQL para Keycloak
  postgres-keycloak:
    image: postgres:15-alpine
    container_name: netredisaside2-postgres-keycloak
    environment:
      POSTGRES_USER: keycloak
      POSTGRES_PASSWORD: keycloak123
      POSTGRES_DB: keycloak
    volumes:
      - postgres_keycloak_data:/var/lib/postgresql/data
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD-SHELL", "pg_isready -U keycloak -d keycloak"]
      interval: 10s
      timeout: 5s
      retries: 5

  # Keycloak com importação automática do Realm
  keycloak:
    image: quay.io/keycloak/keycloak:23.0
    container_name: netredisaside2-keycloak
    command:
      - start-dev
      - --import-realm
    environment:
      KC_DB: postgres
      KC_DB_URL: jdbc:postgresql://postgres-keycloak:5432/keycloak
      KC_DB_USERNAME: keycloak
      KC_DB_PASSWORD: keycloak123
      KC_HOSTNAME: localhost
      KC_HOSTNAME_PORT: 8080
      KC_HOSTNAME_STRICT: false
      KC_HOSTNAME_STRICT_HTTPS: false
      KC_LOG_LEVEL: info
      KC_METRICS_ENABLED: true
      KC_HEALTH_ENABLED: true
      KEYCLOAK_ADMIN: admin
      KEYCLOAK_ADMIN_PASSWORD: admin123
    volumes:
      - ./keycloak/realm-netredisaside2.json:/opt/keycloak/data/import/realm-netredisaside2.json
    ports:
      - "8080:8080"
    depends_on:
      postgres-keycloak:
        condition: service_healthy
    networks:
      - netredisaside2-network
    healthcheck:
      test: ["CMD-SHELL", "exec 3<>/dev/tcp/127.0.0.1/8080;echo -e 'GET /health/ready HTTP/1.1\r\nhost: 127.0.0.1\r\nConnection: close\r\n\r\n' >&3;if [ $? -eq 0 ]; then echo 'Healthcheck Successful';exit 0;else echo 'Healthcheck Failed';exit 1;fi;"]
      interval: 30s
      timeout: 10s
      retries: 10
      start_period: 60s

networks:
  netredisaside2-network:
    driver: bridge

volumes:
  postgres_data:
  postgres_keycloak_data:
  redis_data:
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
    "Audience": "account",
    "MetadataAddress": "http://localhost:8080/realms/netredisaside2/.well-known/openid-configuration",
    "RequireHttpsMetadata": false,
    "ValidateAudience": false,
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
        options.MetadataAddress = keycloakOptions.MetadataAddress;
        options.RequireHttpsMetadata = keycloakOptions.RequireHttpsMetadata;
        
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = keycloakOptions.ValidateAudience,
            ValidateIssuer = keycloakOptions.ValidateIssuer,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = keycloakOptions.Authority,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Program>>();
                logger.LogError("Authentication failed: {Error}", context.Exception.Message);
                return Task.CompletedTask;
            }
        };
    });

// Configurar Autorização
builder.Services.AddAuthorization(options =>
{
    AuthorizationPolicies.ConfigurePolicies(options);
});

// Registrar serviços
builder.Services.AddScoped<ICacheHelper, CacheHelper>();
builder.Services.AddScoped<IAssuntoRepository, AssuntoRepository>();
builder.Services.AddScoped<IMovimentacaoRepository, MovimentacaoRepository>();
builder.Services.AddScoped<ITipoDocumentoRepository, TipoDocumentoRepository>();
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
        Description = "API de Gestão Documental com Keycloak + PostgreSQL + Redis"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header. Exemplo: \"Bearer {token}\"",
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

app.UseAuthentication();
app.UseAuthorization();

// Mapear endpoints
app.MapAssuntoEndpoints();
app.MapMovimentacaoEndpoints();
app.MapTipoDocumentoEndpoints();
app.MapCacheEndpoints();

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    timestamp = DateTime.UtcNow,
    application = "NetRedisASide2",
    keycloak = "http://localhost:8080"
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
    public string MetadataAddress { get; set; } = string.Empty;
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
        options.AddPolicy(AdminPolicy, policy =>
            policy.RequireRole("admin"));

        options.AddPolicy(UserPolicy, policy =>
            policy.RequireRole("admin", "manager", "user"));

        options.AddPolicy(ManagerPolicy, policy =>
            policy.RequireRole("admin", "manager"));
    }
}

// ============================================
// Helpers/ICacheHelper.cs, CacheHelper.cs
// Repositories e Services
// (Código completo igual aos exemplos anteriores)
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
        .WithName("CreateAssunto")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        group.MapPut("/{id}", async (int id, Assunto assunto, IAssuntoService service) =>
        {
            var result = await service.UpdateAsync(id, assunto);
            return result.Success ? Results.Ok(result) : Results.NotFound(result);
        })
        .WithName("UpdateAssunto")
        .RequireAuthorization(AuthorizationPolicies.ManagerPolicy);

        group.MapDelete("/{id}", async (int id, IAssuntoService service) =>
        {
            var result = await service.DeleteAsync(id);
            return result.Success ? Results.NoContent() : Results.NotFound(result);
        })
        .WithName("DeleteAssunto")
        .RequireAuthorization(AuthorizationPolicies.AdminPolicy);
    }
}

// Endpoints de Movimentacao e TipoDocumento seguem o mesmo padrão

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
// README.md - GUIA COMPLETO
// ============================================
/*
# NetRedisASide2

Sistema de Gestão Documental com autenticação Keycloak, PostgreSQL+pgvector e Redis.

## 🚀 Início Rápido

### 1. Pré-requisitos
- .NET 8 SDK
- Docker e Docker Compose

### 2. Estrutura de Pastas
```
mkdir NetRedisASide2
cd NetRedisASide2
mkdir keycloak Models Data Repositories Services Helpers Authentication Endpoints
```

### 3. Criar arquivo do Realm
Criar arquivo `keycloak/realm-netredisaside2.json` com o conteúdo fornecido acima.

### 4. Subir containers
```bash
docker-compose up -d

# Aguardar ~60 segundos para Keycloak importar o realm
docker-compose logs -f keycloak
```

### 5. Instalar pacotes
```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
dotnet add package Pgvector.EntityFrameworkCore
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
dotnet add package Swashbuckle.AspNetCore
```

### 6. Aplicar migrations
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 7. Executar aplicação
```bash
dotnet run
```

## 🔐 Usuários Pré-configurados

| Usuário | Senha | Role | Permissões |
|---------|-------|------|------------|
| admin | admin123 | admin | Total |
| manager | manager123 | manager | Criar/Editar |
| user | user123 | user | Apenas Leitura |

## 🧪 Testando a API

### 1. Obter Token JWT

**Admin:**
```bash
curl -X POST 'http://localhost:8080/realms/netredisaside2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'username=admin' \
  -d 'password=admin123' \
  -d 'grant_type=password'
```

**Manager:**
```bash
curl -X POST 'http://localhost:8080/realms/netredisaside2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'username=manager' \
  -d 'password=manager123' \
  -d 'grant_type=password'
```

**User:**
```bash
curl -X POST 'http://localhost:8080/realms/netredisaside2/protocol/openid-connect/token' \
  -H 'Content-Type: application/x-www-form-urlencoded' \
  -d 'client_id=netredisaside2-api' \
  -d 'username=user' \
  -d 'password=user123' \
  -d 'grant_type=password'
```

### 2. Usar Token

```bash
# Salvar token
TOKEN="eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9..."

# Listar assuntos
curl -H "Authorization: Bearer $TOKEN" \
  http://localhost:5000/api/assuntos

# Criar assunto (manager ou admin)
curl -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -X POST http://localhost:5000/api/assuntos \
  -d '{"nome":"Compras","descricao":"Processos de compra"}'

# Deletar (apenas admin)
curl -H "Authorization: Bearer $TOKEN" \
  -X DELETE http://localhost:5000/api/assuntos/1
```

## 📊 Endpoints

### Assuntos
- GET /api/assuntos - Listar (user, manager, admin)
- GET /api/assuntos/{id} - Buscar (user, manager, admin)
- POST /api/assuntos - Criar (manager, admin)
- PUT /api/assuntos/{id} - Atualizar (manager, admin)
- DELETE /api/assuntos/{id} - Deletar (admin)

### Movimentações
- GET /api/movimentacoes - Listar (user, manager, admin)
- GET /api/movimentacoes/{id} - Buscar (user, manager, admin)
- POST /api/movimentacoes - Criar (manager, admin)
- PUT /api/movimentacoes/{id} - Atualizar (manager, admin)
- DELETE /api/movimentacoes/{id} - Deletar (admin)

### Tipos de Documento
- GET /api/tipos-documento - Listar (user, manager, admin)
- GET /api/tipos-documento/{id} - Buscar (user, manager, admin)
- POST /api/tipos-documento - Criar (manager, admin)
- PUT /api/tipos-documento/{id} - Atualizar (manager, admin)
- DELETE /api/tipos-documento/{id} - Deletar (admin)

### Cache
- GET /api/cache/info - Informações (admin)
- DELETE /api/cache/clear - Limpar (admin)

## 🔍 Verificar Serviços

### Keycloak
```bash
# Admin Console
http://localhost:8080

# Realm: netredisaside2
# Admin: admin / admin123
```

### PostgreSQL
```bash
docker exec -it netredisaside2-postgres psql -U admin -d netredisaside2

SELECT * FROM "Assuntos";
\q
```

### Redis
```bash
docker exec -it netredisaside2-redis redis-cli

KEYS NetRedisASide2:*
exit
```

## 🛠️ Troubleshooting

### Keycloak não importa o realm
```bash
# Ver logs
docker-compose logs keycloak

# Verificar se arquivo existe
ls -la keycloak/realm-netredisaside2.json

# Recriar containers
docker-compose down -v
docker-compose up -d
```

### Token inválido
```bash
# Verificar se Keycloak está pronto
curl http://localhost:8080/health/ready

# Obter novo token
curl -X POST 'http://localhost:8080/realms/netredisaside2/protocol/openid-connect/token' ...
```

## 📝 Swagger

Acesse: http://localhost:5000/swagger

1. Obtenha um token JWT
2. Clique em "Authorize" 🔒
3. Cole o token
4. Teste os endpoints!

## 🏗️ Arquitetura

```
Client
  ↓
  POST /token (Keycloak) → JWT Token
  ↓
  API Request + Bearer Token
  ↓
JWT Validation → Roles Extraction
  ↓
Authorization Policy Check
  ↓
Endpoint (AssuntoEndpoints.cs)
  ↓
Service (AssuntoService.cs) + Cache (Redis)
  ↓
Repository (AssuntoRepository.cs)
  ↓
EF Core → PostgreSQL + pgvector
```

## 🎯 Tecnologias

- .NET 8
- Keycloak 23.0 (JWT)
- PostgreSQL 15 + pgvector
- Redis 7
- Entity Framework Core
- IDistributedCache
- Minimal API

## 📦 Volumes Docker

- postgres_data: Dados do PostgreSQL
- postgres_keycloak_data: Dados do Keycloak
- redis_data: Dados do Redis

## 🔄 Comandos Úteis

```bash
# Parar tudo
docker-compose down

# Parar e remover volumes
docker-compose down -v

# Ver logs
docker-compose logs -f

# Reiniciar Keycloak
docker-compose restart keycloak

# Recriar migrations
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```
*/