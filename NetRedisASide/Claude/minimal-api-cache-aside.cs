// ============================================
// Program.cs - Minimal API com Cache Aside
// ============================================
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configurar Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse("localhost:6379", true);
    configuration.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(configuration);
});

// Registrar serviços
builder.Services.AddScoped<ICacheService, RedisCacheService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ============================================
// ENDPOINTS - PRODUCTS
// ============================================

// GET - Buscar produto por ID (Cache Aside)
app.MapGet("/api/products/{id}", async (
    int id, 
    IProductRepository repository, 
    ICacheService cache) =>
{
    var cacheKey = $"product:{id}";
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // 1. Tentar buscar do cache
    var cachedProduct = await cache.GetAsync<Product>(cacheKey);
    
    if (cachedProduct != null)
    {
        stopwatch.Stop();
        return Results.Ok(new
        {
            data = cachedProduct,
            source = "CACHE (Redis)",
            responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
            cached = true
        });
    }

    // 2. Cache MISS - buscar do banco
    var product = await repository.GetByIdAsync(id);
    
    if (product == null)
    {
        return Results.NotFound(new { message = "Produto não encontrado" });
    }

    // 3. Armazenar no cache (10 minutos)
    await cache.SetAsync(cacheKey, product, TimeSpan.FromMinutes(10));
    
    stopwatch.Stop();
    return Results.Ok(new
    {
        data = product,
        source = "DATABASE (primeira chamada)",
        responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
        cached = false,
        message = "Dados armazenados no cache por 10 minutos"
    });
})
.WithName("GetProduct")
.WithOpenApi();

// GET - Listar todos os produtos (Cache Aside)
app.MapGet("/api/products", async (
    IProductRepository repository, 
    ICacheService cache) =>
{
    var cacheKey = "products:all";
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // 1. Tentar buscar do cache
    var cachedProducts = await cache.GetAsync<List<Product>>(cacheKey);
    
    if (cachedProducts != null)
    {
        stopwatch.Stop();
        return Results.Ok(new
        {
            data = cachedProducts,
            count = cachedProducts.Count,
            source = "CACHE (Redis)",
            responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
        });
    }

    // 2. Cache MISS - buscar do banco
    var products = await repository.GetAllAsync();
    
    // 3. Armazenar no cache (5 minutos)
    await cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(5));
    
    stopwatch.Stop();
    return Results.Ok(new
    {
        data = products,
        count = products.Count,
        source = "DATABASE",
        responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
        message = "Lista cacheada por 5 minutos"
    });
})
.WithName("GetAllProducts")
.WithOpenApi();

// GET - Buscar produtos por categoria (Cache Aside com parâmetro)
app.MapGet("/api/products/category/{category}", async (
    string category,
    IProductRepository repository, 
    ICacheService cache) =>
{
    var cacheKey = $"products:category:{category.ToLower()}";
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // 1. Tentar buscar do cache
    var cachedProducts = await cache.GetAsync<List<Product>>(cacheKey);
    
    if (cachedProducts != null)
    {
        stopwatch.Stop();
        return Results.Ok(new
        {
            category = category,
            data = cachedProducts,
            count = cachedProducts.Count,
            source = "CACHE",
            responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
        });
    }

    // 2. Cache MISS - buscar do banco
    var products = await repository.GetByCategoryAsync(category);
    
    // 3. Armazenar no cache (3 minutos)
    await cache.SetAsync(cacheKey, products, TimeSpan.FromMinutes(3));
    
    stopwatch.Stop();
    return Results.Ok(new
    {
        category = category,
        data = products,
        count = products.Count,
        source = "DATABASE",
        responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
    });
})
.WithName("GetProductsByCategory")
.WithOpenApi();

// POST - Criar produto (invalida cache)
app.MapPost("/api/products", async (
    Product product,
    IProductRepository repository, 
    ICacheService cache) =>
{
    var created = await repository.CreateAsync(product);
    
    // Invalidar caches relacionados
    await cache.RemoveAsync("products:all");
    await cache.RemoveAsync($"products:category:{product.Category.ToLower()}");
    
    return Results.Created($"/api/products/{created.Id}", new
    {
        message = "Produto criado com sucesso",
        data = created,
        cacheInvalidated = new[] { "products:all", $"products:category:{product.Category}" }
    });
})
.WithName("CreateProduct")
.WithOpenApi();

// PUT - Atualizar produto (invalida cache)
app.MapPut("/api/products/{id}", async (
    int id,
    Product product,
    IProductRepository repository, 
    ICacheService cache) =>
{
    product.Id = id;
    var updated = await repository.UpdateAsync(product);
    
    if (updated == null)
    {
        return Results.NotFound(new { message = "Produto não encontrado" });
    }

    // Invalidar caches relacionados
    await cache.RemoveAsync($"product:{id}");
    await cache.RemoveAsync("products:all");
    await cache.RemoveAsync($"products:category:{product.Category.ToLower()}");
    
    return Results.Ok(new
    {
        message = "Produto atualizado com sucesso",
        data = updated,
        cacheInvalidated = true
    });
})
.WithName("UpdateProduct")
.WithOpenApi();

// DELETE - Deletar produto (invalida cache)
app.MapDelete("/api/products/{id}", async (
    int id,
    IProductRepository repository, 
    ICacheService cache) =>
{
    var product = await repository.GetByIdAsync(id);
    if (product == null)
    {
        return Results.NotFound(new { message = "Produto não encontrado" });
    }

    var deleted = await repository.DeleteAsync(id);
    
    if (deleted)
    {
        // Invalidar todos os caches relacionados
        await cache.RemoveAsync($"product:{id}");
        await cache.RemoveAsync("products:all");
        await cache.RemoveAsync($"products:category:{product.Category.ToLower()}");
        
        return Results.Ok(new
        {
            message = "Produto deletado com sucesso",
            cacheInvalidated = true
        });
    }

    return Results.BadRequest(new { message = "Erro ao deletar produto" });
})
.WithName("DeleteProduct")
.WithOpenApi();

// ============================================
// ENDPOINTS - ORDERS (Exemplo adicional)
// ============================================

// GET - Buscar pedido por ID com cache
app.MapGet("/api/orders/{id}", async (
    int id,
    IOrderRepository repository, 
    ICacheService cache) =>
{
    var cacheKey = $"order:{id}";
    
    // Cache Aside Pattern
    var cachedOrder = await cache.GetAsync<Order>(cacheKey);
    if (cachedOrder != null)
    {
        return Results.Ok(new { data = cachedOrder, source = "CACHE" });
    }

    var order = await repository.GetByIdAsync(id);
    if (order == null)
    {
        return Results.NotFound();
    }

    await cache.SetAsync(cacheKey, order, TimeSpan.FromMinutes(15));
    return Results.Ok(new { data = order, source = "DATABASE" });
})
.WithName("GetOrder")
.WithOpenApi();

// GET - Buscar pedidos por usuário
app.MapGet("/api/orders/user/{userId}", async (
    int userId,
    IOrderRepository repository, 
    ICacheService cache) =>
{
    var cacheKey = $"orders:user:{userId}";
    
    var cachedOrders = await cache.GetAsync<List<Order>>(cacheKey);
    if (cachedOrders != null)
    {
        return Results.Ok(new { data = cachedOrders, count = cachedOrders.Count, source = "CACHE" });
    }

    var orders = await repository.GetByUserIdAsync(userId);
    await cache.SetAsync(cacheKey, orders, TimeSpan.FromMinutes(5));
    
    return Results.Ok(new { data = orders, count = orders.Count, source = "DATABASE" });
})
.WithName("GetOrdersByUser")
.WithOpenApi();

// ============================================
// ENDPOINTS - CACHE MANAGEMENT
// ============================================

// GET - Verificar se uma chave existe no cache
app.MapGet("/api/cache/exists/{key}", async (string key, ICacheService cache) =>
{
    var exists = await cache.ExistsAsync(key);
    return Results.Ok(new { key = key, exists = exists });
})
.WithName("CheckCacheKey")
.WithOpenApi();

// DELETE - Limpar cache específico
app.MapDelete("/api/cache/{key}", async (string key, ICacheService cache) =>
{
    await cache.RemoveAsync(key);
    return Results.Ok(new { message = $"Cache '{key}' removido com sucesso" });
})
.WithName("ClearCache")
.WithOpenApi();

// DELETE - Limpar todo o cache de produtos
app.MapDelete("/api/cache/products/all", async (ICacheService cache) =>
{
    var keys = new[] 
    { 
        "products:all", 
        "products:category:electronics",
        "products:category:books",
        "products:category:clothing"
    };

    foreach (var key in keys)
    {
        await cache.RemoveAsync(key);
    }

    return Results.Ok(new 
    { 
        message = "Cache de produtos limpo",
        keysRemoved = keys
    });
})
.WithName("ClearAllProductsCache")
.WithOpenApi();

// GET - Estatísticas do cache (demonstração)
app.MapGet("/api/cache/stats", async (ICacheService cache) =>
{
    var productKeys = new[] { "products:all", "product:1", "product:2", "product:3" };
    var stats = new Dictionary<string, bool>();

    foreach (var key in productKeys)
    {
        stats[key] = await cache.ExistsAsync(key);
    }

    return Results.Ok(new
    {
        message = "Status do cache",
        keys = stats,
        timestamp = DateTime.Now
    });
})
.WithName("GetCacheStats")
.WithOpenApi();

app.Run();

// ============================================
// MODELS
// ============================================

public record Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

public record Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public decimal Total { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public List<OrderItem> Items { get; set; } = new();
}

public record OrderItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

// ============================================
// CACHE SERVICE
// ============================================

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task<bool> ExistsAsync(string key);
}

public class RedisCacheService : ICacheService
{
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;

    public RedisCacheService(IConnectionMultiplexer redis, ILogger<RedisCacheService> logger)
    {
        _database = redis.GetDatabase();
        _logger = logger;
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var value = await _database.StringGetAsync(key);
            
            if (value.IsNullOrEmpty)
            {
                _logger.LogInformation("Cache MISS: {Key}", key);
                return default;
            }

            _logger.LogInformation("Cache HIT: {Key}", key);
            return JsonSerializer.Deserialize<T>(value!);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao buscar do cache: {Key}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
    {
        try
        {
            var serializedValue = JsonSerializer.Serialize(value);
            await _database.StringSetAsync(key, serializedValue, expiration);
            _logger.LogInformation("Cache SET: {Key} (expira em {Expiration})", key, expiration);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gravar no cache: {Key}", key);
        }
    }

    public async Task RemoveAsync(string key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
            _logger.LogInformation("Cache REMOVED: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao remover do cache: {Key}", key);
        }
    }

    public async Task<bool> ExistsAsync(string key)
    {
        try
        {
            return await _database.KeyExistsAsync(key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao verificar existência no cache: {Key}", key);
            return false;
        }
    }
}

// ============================================
// REPOSITORIES
// ============================================

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task<List<Product>> GetByCategoryAsync(string category);
    Task<Product> CreateAsync(Product product);
    Task<Product?> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
}

public class ProductRepository : IProductRepository
{
    private static readonly List<Product> _products = new()
    {
        new() { Id = 1, Name = "Notebook Dell", Description = "i7, 16GB RAM", Category = "Electronics", Price = 3500.00m, Stock = 10 },
        new() { Id = 2, Name = "Mouse Logitech", Description = "Mouse sem fio", Category = "Electronics", Price = 150.00m, Stock = 50 },
        new() { Id = 3, Name = "Clean Code", Description = "Livro sobre código limpo", Category = "Books", Price = 80.00m, Stock = 30 },
        new() { Id = 4, Name = "Camiseta", Description = "Camiseta 100% algodão", Category = "Clothing", Price = 50.00m, Stock = 100 },
        new() { Id = 5, Name = "Teclado Mecânico", Description = "RGB, switches blue", Category = "Electronics", Price = 450.00m, Stock = 25 }
    };

    public Task<Product?> GetByIdAsync(int id)
    {
        // Simula delay de banco de dados
        Thread.Sleep(1000);
        return Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }

    public Task<List<Product>> GetAllAsync()
    {
        Thread.Sleep(1500);
        return Task.FromResult(_products.ToList());
    }

    public Task<List<Product>> GetByCategoryAsync(string category)
    {
        Thread.Sleep(800);
        var filtered = _products.Where(p => 
            p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        return Task.FromResult(filtered);
    }

    public Task<Product> CreateAsync(Product product)
    {
        product.Id = _products.Max(p => p.Id) + 1;
        _products.Add(product);
        return Task.FromResult(product);
    }

    public Task<Product?> UpdateAsync(Product product)
    {
        var existing = _products.FirstOrDefault(p => p.Id == product.Id);
        if (existing != null)
        {
            existing.Name = product.Name;
            existing.Description = product.Description;
            existing.Category = product.Category;
            existing.Price = product.Price;
            existing.Stock = product.Stock;
        }
        return Task.FromResult(existing);
    }

    public Task<bool> DeleteAsync(int id)
    {
        var product = _products.FirstOrDefault(p => p.Id == id);
        if (product != null)
        {
            _products.Remove(product);
            return Task.FromResult(true);
        }
        return Task.FromResult(false);
    }
}

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(int id);
    Task<List<Order>> GetByUserIdAsync(int userId);
}

public class OrderRepository : IOrderRepository
{
    private static readonly List<Order> _orders = new()
    {
        new() 
        { 
            Id = 1, 
            UserId = 1, 
            CustomerName = "João Silva", 
            Total = 3650.00m, 
            Status = "Entregue",
            Items = new() 
            {
                new() { ProductId = 1, ProductName = "Notebook Dell", Quantity = 1, Price = 3500.00m },
                new() { ProductId = 2, ProductName = "Mouse Logitech", Quantity = 1, Price = 150.00m }
            }
        }
    };

    public Task<Order?> GetByIdAsync(int id)
    {
        Thread.Sleep(1200);
        return Task.FromResult(_orders.FirstOrDefault(o => o.Id == id));
    }

    public Task<List<Order>> GetByUserIdAsync(int userId)
    {
        Thread.Sleep(1000);
        return Task.FromResult(_orders.Where(o => o.UserId == userId).ToList());
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
    <PackageReference Include="StackExchange.Redis" Version="2.8.16" />
    <PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
  </ItemGroup>
</Project>
*/