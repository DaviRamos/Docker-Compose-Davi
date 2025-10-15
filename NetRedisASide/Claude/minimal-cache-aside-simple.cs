// ============================================
// Program.cs - Minimal API Puro com Cache Aside
// ============================================
using StackExchange.Redis;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Configurar Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = ConfigurationOptions.Parse("localhost:6379", true);
    config.AbortOnConnectFail = false;
    return ConnectionMultiplexer.Connect(config);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ============================================
// DADOS EM MEMÓRIA (simulando banco de dados)
// ============================================

var products = new List<Product>
{
    new(1, "Notebook Dell", "i7, 16GB RAM", "Electronics", 3500.00m, 10),
    new(2, "Mouse Logitech", "Mouse sem fio", "Electronics", 150.00m, 50),
    new(3, "Teclado Mecânico", "RGB, switches blue", "Electronics", 450.00m, 25),
    new(4, "Clean Code", "Livro sobre código limpo", "Books", 80.00m, 30),
    new(5, "Design Patterns", "Padrões de projeto", "Books", 95.00m, 20),
    new(6, "Camiseta", "100% algodão", "Clothing", 50.00m, 100),
    new(7, "Calça Jeans", "Jeans premium", "Clothing", 120.00m, 60),
    new(8, "Headset Gamer", "7.1 surround", "Electronics", 350.00m, 15)
};

var orders = new List<Order>
{
    new(1, 1, "João Silva", 3650.00m, "Entregue", DateTime.Now.AddDays(-5),
        new() { new(1, "Notebook Dell", 1, 3500.00m), new(2, "Mouse Logitech", 1, 150.00m) }),
    new(2, 2, "Maria Santos", 450.00m, "Processando", DateTime.Now.AddDays(-2),
        new() { new(3, "Teclado Mecânico", 1, 450.00m) }),
    new(3, 1, "João Silva", 80.00m, "Enviado", DateTime.Now.AddDays(-1),
        new() { new(4, "Clean Code", 1, 80.00m) })
};

// ============================================
// ENDPOINTS - PRODUCTS (Cache Aside Pattern)
// ============================================

// GET - Buscar produto por ID (CACHE ASIDE)
app.MapGet("/api/products/{id}", async (int id, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = $"product:{id}";
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // 1️⃣ PASSO 1: Tentar buscar do CACHE
    var cachedValue = await db.StringGetAsync(cacheKey);
    
    if (!cachedValue.IsNullOrEmpty)
    {
        var cachedProduct = JsonSerializer.Deserialize<Product>(cachedValue!);
        stopwatch.Stop();
        
        app.Logger.LogInformation("✅ CACHE HIT: {Key}", cacheKey);
        
        return Results.Ok(new
        {
            data = cachedProduct,
            source = "🎯 REDIS CACHE",
            responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
            cacheHit = true
        });
    }

    app.Logger.LogWarning("❌ CACHE MISS: {Key}", cacheKey);

    // 2️⃣ PASSO 2: CACHE MISS - Buscar dos dados (simulando banco)
    Thread.Sleep(1000); // Simula delay do banco
    var product = products.FirstOrDefault(p => p.Id == id);
    
    if (product == null)
    {
        return Results.NotFound(new { message = "Produto não encontrado" });
    }

    // 3️⃣ PASSO 3: Armazenar no CACHE (10 minutos)
    var jsonProduct = JsonSerializer.Serialize(product);
    await db.StringSetAsync(cacheKey, jsonProduct, TimeSpan.FromMinutes(10));
    
    stopwatch.Stop();
    app.Logger.LogInformation("💾 Produto {Id} armazenado no cache", id);

    return Results.Ok(new
    {
        data = product,
        source = "💽 DATABASE (primeira chamada)",
        responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
        cacheHit = false,
        message = "Dados salvos no cache por 10 minutos"
    });
})
.WithName("GetProduct")
.WithOpenApi();

// GET - Listar todos os produtos (CACHE ASIDE)
app.MapGet("/api/products", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = "products:all";
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // 1️⃣ Tentar buscar do cache
    var cachedValue = await db.StringGetAsync(cacheKey);
    
    if (!cachedValue.IsNullOrEmpty)
    {
        var cachedProducts = JsonSerializer.Deserialize<List<Product>>(cachedValue!);
        stopwatch.Stop();
        
        app.Logger.LogInformation("✅ CACHE HIT: {Key}", cacheKey);
        
        return Results.Ok(new
        {
            data = cachedProducts,
            count = cachedProducts!.Count,
            source = "🎯 REDIS CACHE",
            responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
        });
    }

    app.Logger.LogWarning("❌ CACHE MISS: {Key}", cacheKey);

    // 2️⃣ Buscar dos dados
    Thread.Sleep(1500); // Simula consulta pesada
    var allProducts = products.ToList();

    // 3️⃣ Armazenar no cache (5 minutos)
    var jsonProducts = JsonSerializer.Serialize(allProducts);
    await db.StringSetAsync(cacheKey, jsonProducts, TimeSpan.FromMinutes(5));
    
    stopwatch.Stop();
    app.Logger.LogInformation("💾 Lista de produtos armazenada no cache");

    return Results.Ok(new
    {
        data = allProducts,
        count = allProducts.Count,
        source = "💽 DATABASE",
        responseTime = $"{stopwatch.ElapsedMilliseconds}ms",
        message = "Lista cacheada por 5 minutos"
    });
})
.WithName("GetAllProducts")
.WithOpenApi();

// GET - Produtos por categoria (CACHE ASIDE com variação)
app.MapGet("/api/products/category/{category}", async (string category, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = $"products:category:{category.ToLower()}";
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // 1️⃣ Tentar cache
    var cachedValue = await db.StringGetAsync(cacheKey);
    
    if (!cachedValue.IsNullOrEmpty)
    {
        var cachedProducts = JsonSerializer.Deserialize<List<Product>>(cachedValue!);
        stopwatch.Stop();
        
        return Results.Ok(new
        {
            category = category,
            data = cachedProducts,
            count = cachedProducts!.Count,
            source = "🎯 CACHE",
            responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
        });
    }

    // 2️⃣ Buscar e filtrar
    Thread.Sleep(800);
    var filtered = products
        .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
        .ToList();

    // 3️⃣ Cachear (3 minutos)
    var json = JsonSerializer.Serialize(filtered);
    await db.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(3));
    
    stopwatch.Stop();

    return Results.Ok(new
    {
        category = category,
        data = filtered,
        count = filtered.Count,
        source = "💽 DATABASE",
        responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
    });
})
.WithName("GetProductsByCategory")
.WithOpenApi();

// GET - Buscar produtos (CACHE ASIDE com múltiplos parâmetros)
app.MapGet("/api/products/search", async (
    string? name, 
    decimal? minPrice, 
    decimal? maxPrice,
    IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = $"products:search:{name}:{minPrice}:{maxPrice}";
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    // 1️⃣ Tentar cache
    var cachedValue = await db.StringGetAsync(cacheKey);
    
    if (!cachedValue.IsNullOrEmpty)
    {
        var cachedProducts = JsonSerializer.Deserialize<List<Product>>(cachedValue!);
        stopwatch.Stop();
        
        return Results.Ok(new
        {
            filters = new { name, minPrice, maxPrice },
            data = cachedProducts,
            count = cachedProducts!.Count,
            source = "🎯 CACHE",
            responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
        });
    }

    // 2️⃣ Buscar e filtrar
    Thread.Sleep(600);
    var filtered = products.AsEnumerable();

    if (!string.IsNullOrEmpty(name))
        filtered = filtered.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    
    if (minPrice.HasValue)
        filtered = filtered.Where(p => p.Price >= minPrice.Value);
    
    if (maxPrice.HasValue)
        filtered = filtered.Where(p => p.Price <= maxPrice.Value);

    var result = filtered.ToList();

    // 3️⃣ Cachear (2 minutos)
    var json = JsonSerializer.Serialize(result);
    await db.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(2));
    
    stopwatch.Stop();

    return Results.Ok(new
    {
        filters = new { name, minPrice, maxPrice },
        data = result,
        count = result.Count,
        source = "💽 DATABASE",
        responseTime = $"{stopwatch.ElapsedMilliseconds}ms"
    });
})
.WithName("SearchProducts")
.WithOpenApi();

// GET - Top produtos (Cache Aside)
app.MapGet("/api/products/top/{limit}", async (int limit, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = $"products:top:{limit}";

    // 1️⃣ Cache
    var cachedValue = await db.StringGetAsync(cacheKey);
    if (!cachedValue.IsNullOrEmpty)
    {
        var cached = JsonSerializer.Deserialize<List<Product>>(cachedValue!);
        return Results.Ok(new { data = cached, source = "🎯 CACHE" });
    }

    // 2️⃣ Buscar
    Thread.Sleep(700);
    var top = products.OrderByDescending(p => p.Price).Take(limit).ToList();

    // 3️⃣ Cachear
    await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(top), TimeSpan.FromMinutes(5));

    return Results.Ok(new { data = top, count = top.Count, source = "💽 DATABASE" });
})
.WithName("GetTopProducts")
.WithOpenApi();

// POST - Criar produto (INVALIDA CACHE)
app.MapPost("/api/products", async (Product product, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    
    // Criar produto
    var newProduct = product with { Id = products.Max(p => p.Id) + 1 };
    products.Add(newProduct);

    // ⚠️ INVALIDAR CACHES relacionados
    var keysToInvalidate = new[]
    {
        "products:all",
        $"products:category:{newProduct.Category.ToLower()}",
        "products:top:5",
        "products:top:10"
    };

    foreach (var key in keysToInvalidate)
    {
        await db.KeyDeleteAsync(key);
        app.Logger.LogInformation("🗑️ Cache invalidado: {Key}", key);
    }

    return Results.Created($"/api/products/{newProduct.Id}", new
    {
        message = "Produto criado com sucesso",
        data = newProduct,
        cacheInvalidated = keysToInvalidate
    });
})
.WithName("CreateProduct")
.WithOpenApi();

// PUT - Atualizar produto (INVALIDA CACHE)
app.MapPut("/api/products/{id}", async (int id, Product updatedProduct, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
        return Results.NotFound(new { message = "Produto não encontrado" });

    // Atualizar
    products.Remove(product);
    var updated = updatedProduct with { Id = id };
    products.Add(updated);

    // ⚠️ INVALIDAR múltiplos caches
    var keysToInvalidate = new[]
    {
        $"product:{id}",
        "products:all",
        $"products:category:{product.Category.ToLower()}",
        $"products:category:{updated.Category.ToLower()}"
    };

    foreach (var key in keysToInvalidate)
    {
        await db.KeyDeleteAsync(key);
    }

    return Results.Ok(new
    {
        message = "Produto atualizado",
        data = updated,
        cacheInvalidated = keysToInvalidate
    });
})
.WithName("UpdateProduct")
.WithOpenApi();

// DELETE - Deletar produto (INVALIDA CACHE)
app.MapDelete("/api/products/{id}", async (int id, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    
    var product = products.FirstOrDefault(p => p.Id == id);
    if (product == null)
        return Results.NotFound();

    products.Remove(product);

    // ⚠️ INVALIDAR caches
    var keysToInvalidate = new[]
    {
        $"product:{id}",
        "products:all",
        $"products:category:{product.Category.ToLower()}"
    };

    foreach (var key in keysToInvalidate)
    {
        await db.KeyDeleteAsync(key);
    }

    return Results.NoContent();
})
.WithName("DeleteProduct")
.WithOpenApi();

// ============================================
// ENDPOINTS - ORDERS (Cache Aside)
// ============================================

// GET - Buscar pedido (Cache Aside)
app.MapGet("/api/orders/{id}", async (int id, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = $"order:{id}";

    // Cache Aside
    var cached = await db.StringGetAsync(cacheKey);
    if (!cached.IsNullOrEmpty)
    {
        var order = JsonSerializer.Deserialize<Order>(cached!);
        return Results.Ok(new { data = order, source = "🎯 CACHE" });
    }

    Thread.Sleep(1200);
    var dbOrder = orders.FirstOrDefault(o => o.Id == id);
    
    if (dbOrder == null)
        return Results.NotFound();

    await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(dbOrder), TimeSpan.FromMinutes(15));

    return Results.Ok(new { data = dbOrder, source = "💽 DATABASE" });
})
.WithName("GetOrder")
.WithOpenApi();

// GET - Pedidos por usuário (Cache Aside)
app.MapGet("/api/orders/user/{userId}", async (int userId, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = $"orders:user:{userId}";

    var cached = await db.StringGetAsync(cacheKey);
    if (!cached.IsNullOrEmpty)
    {
        var userOrders = JsonSerializer.Deserialize<List<Order>>(cached!);
        return Results.Ok(new { data = userOrders, count = userOrders!.Count, source = "🎯 CACHE" });
    }

    Thread.Sleep(900);
    var dbOrders = orders.Where(o => o.UserId == userId).ToList();

    await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(dbOrders), TimeSpan.FromMinutes(10));

    return Results.Ok(new { data = dbOrders, count = dbOrders.Count, source = "💽 DATABASE" });
})
.WithName("GetOrdersByUser")
.WithOpenApi();

// GET - Todos pedidos (Cache Aside)
app.MapGet("/api/orders", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var cacheKey = "orders:all";

    var cached = await db.StringGetAsync(cacheKey);
    if (!cached.IsNullOrEmpty)
    {
        var allOrders = JsonSerializer.Deserialize<List<Order>>(cached!);
        return Results.Ok(new { data = allOrders, count = allOrders!.Count, source = "🎯 CACHE" });
    }

    Thread.Sleep(1100);
    var dbOrders = orders.ToList();

    await db.StringSetAsync(cacheKey, JsonSerializer.Serialize(dbOrders), TimeSpan.FromMinutes(5));

    return Results.Ok(new { data = dbOrders, count = dbOrders.Count, source = "💽 DATABASE" });
})
.WithName("GetAllOrders")
.WithOpenApi();

// ============================================
// ENDPOINTS - CACHE MANAGEMENT
// ============================================

// GET - Verificar se chave existe no cache
app.MapGet("/api/cache/exists/{key}", async (string key, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var exists = await db.KeyExistsAsync(key);
    var ttl = await db.KeyTimeToLiveAsync(key);

    return Results.Ok(new
    {
        key = key,
        exists = exists,
        ttl = ttl?.TotalSeconds,
        message = exists ? $"Chave existe e expira em {ttl?.TotalSeconds:F0}s" : "Chave não existe"
    });
})
.WithName("CheckCacheKey")
.WithOpenApi();

// DELETE - Limpar cache específico
app.MapDelete("/api/cache/{key}", async (string key, IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var deleted = await db.KeyDeleteAsync(key);

    return Results.Ok(new
    {
        message = deleted ? $"Cache '{key}' removido" : $"Cache '{key}' não existe",
        deleted = deleted
    });
})
.WithName("ClearCache")
.WithOpenApi();

// DELETE - Limpar todo cache de produtos
app.MapDelete("/api/cache/products/clear", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var keys = new[]
    {
        "products:all",
        "products:category:electronics",
        "products:category:books",
        "products:category:clothing",
        "products:top:5",
        "products:top:10"
    };

    var count = 0;
    foreach (var key in keys)
    {
        if (await db.KeyDeleteAsync(key))
            count++;
    }

    return Results.Ok(new
    {
        message = "Cache de produtos limpo",
        keysRemoved = count,
        keys = keys
    });
})
.WithName("ClearAllProductsCache")
.WithOpenApi();

// GET - Estatísticas do cache
app.MapGet("/api/cache/stats", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    
    var keys = new[] 
    { 
        "products:all", 
        "product:1", 
        "product:2", 
        "orders:all",
        "products:category:electronics"
    };

    var stats = new Dictionary<string, object>();

    foreach (var key in keys)
    {
        var exists = await db.KeyExistsAsync(key);
        var ttl = await db.KeyTimeToLiveAsync(key);
        
        stats[key] = new
        {
            exists = exists,
            ttl = ttl?.TotalSeconds,
            status = exists ? "✅ Cached" : "❌ Not Cached"
        };
    }

    return Results.Ok(new
    {
        message = "Status do cache",
        timestamp = DateTime.Now,
        keys = stats
    });
})
.WithName("GetCacheStats")
.WithOpenApi();

// GET - Informações do Redis
app.MapGet("/api/redis/info", async (IConnectionMultiplexer redis) =>
{
    var db = redis.GetDatabase();
    var server = redis.GetServer(redis.GetEndPoints().First());
    
    return Results.Ok(new
    {
        isConnected = redis.IsConnected,
        database = db.Database,
        endpoints = redis.GetEndPoints().Select(e => e.ToString()),
        keyCount = await server.DatabaseSizeAsync(),
        info = "Redis está funcionando!"
    });
})
.WithName("GetRedisInfo")
.WithOpenApi();

// Health check
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.Now }))
.WithName("HealthCheck");

app.Run();

// ============================================
// MODELS (Records)
// ============================================

public record Product(
    int Id,
    string Name,
    string Description,
    string Category,
    decimal Price,
    int Stock
);

public record Order(
    int Id,
    int UserId,
    string CustomerName,
    decimal Total,
    string Status,
    DateTime OrderDate,
    List<OrderItem> Items
);

public record OrderItem(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal Price
);

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