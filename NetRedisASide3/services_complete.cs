// Services/AssuntoService.cs
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using NetRedisASide3.Models;
using NetRedisASide3.Repositories;

namespace NetRedisASide3.Services;

public class AssuntoService
{
    private readonly IRepository<Assunto> _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<AssuntoService> _logger;
    private const string CacheKeyPrefix = "assunto:";
    private const string CacheKeyAll = "assuntos:all";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public AssuntoService(
        IRepository<Assunto> repository,
        IDistributedCache cache,
        ILogger<AssuntoService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Assunto>> GetAllAsync()
    {
        var cachedData = await _cache.GetStringAsync(CacheKeyAll);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Retornando assuntos do cache");
            return JsonSerializer.Deserialize<IEnumerable<Assunto>>(cachedData) ?? Enumerable.Empty<Assunto>();
        }

        var assuntos = await _repository.GetAllAsync();
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheExpiration
        };
        
        await _cache.SetStringAsync(
            CacheKeyAll,
            JsonSerializer.Serialize(assuntos),
            options);

        _logger.LogInformation("Assuntos carregados do banco e salvos no cache");
        
        return assuntos;
    }

    public async Task<Assunto?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Retornando assunto {Id} do cache", id);
            return JsonSerializer.Deserialize<Assunto>(cachedData);
        }

        var assunto = await _repository.GetByIdAsync(id);
        
        if (assunto != null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };
            
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(assunto),
                options);

            _logger.LogInformation("Assunto {Id} carregado do banco e salvo no cache", id);
        }
        
        return assunto;
    }

    public async Task<Assunto> AddAsync(Assunto assunto)
    {
        var result = await _repository.AddAsync(assunto);
        
        await _cache.RemoveAsync(CacheKeyAll);
        _logger.LogInformation("Cache de listagem invalidado após criação do assunto {Id}", result.Id);
        
        return result;
    }

    public async Task<Assunto> UpdateAsync(Assunto assunto)
    {
        var result = await _repository.UpdateAsync(assunto);
        
        var cacheKey = $"{CacheKeyPrefix}{assunto.Id}";
        await _cache.RemoveAsync(cacheKey);
        await _cache.RemoveAsync(CacheKeyAll);
        
        _logger.LogInformation("Cache invalidado após atualização do assunto {Id}", assunto.Id);
        
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        
        if (result)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";
            await _cache.RemoveAsync(cacheKey);
            await _cache.RemoveAsync(CacheKeyAll);
            
            _logger.LogInformation("Cache invalidado após exclusão do assunto {Id}", id);
        }
        
        return result;
    }
}

// Services/MovimentacaoService.cs
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using NetRedisASide3.Models;
using NetRedisASide3.Repositories;

namespace NetRedisASide3.Services;

public class MovimentacaoService
{
    private readonly IRepository<Movimentacao> _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<MovimentacaoService> _logger;
    private const string CacheKeyPrefix = "movimentacao:";
    private const string CacheKeyAll = "movimentacoes:all";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public MovimentacaoService(
        IRepository<Movimentacao> repository,
        IDistributedCache cache,
        ILogger<MovimentacaoService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<Movimentacao>> GetAllAsync()
    {
        var cachedData = await _cache.GetStringAsync(CacheKeyAll);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Retornando movimentações do cache");
            return JsonSerializer.Deserialize<IEnumerable<Movimentacao>>(cachedData) ?? Enumerable.Empty<Movimentacao>();
        }

        var movimentacoes = await _repository.GetAllAsync();
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheExpiration
        };
        
        await _cache.SetStringAsync(
            CacheKeyAll,
            JsonSerializer.Serialize(movimentacoes),
            options);

        _logger.LogInformation("Movimentações carregadas do banco e salvas no cache");
        
        return movimentacoes;
    }

    public async Task<Movimentacao?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Retornando movimentação {Id} do cache", id);
            return JsonSerializer.Deserialize<Movimentacao>(cachedData);
        }

        var movimentacao = await _repository.GetByIdAsync(id);
        
        if (movimentacao != null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };
            
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(movimentacao),
                options);

            _logger.LogInformation("Movimentação {Id} carregada do banco e salva no cache", id);
        }
        
        return movimentacao;
    }

    public async Task<Movimentacao> AddAsync(Movimentacao movimentacao)
    {
        var result = await _repository.AddAsync(movimentacao);
        
        await _cache.RemoveAsync(CacheKeyAll);
        _logger.LogInformation("Cache de listagem invalidado após criação da movimentação {Id}", result.Id);
        
        return result;
    }

    public async Task<Movimentacao> UpdateAsync(Movimentacao movimentacao)
    {
        var result = await _repository.UpdateAsync(movimentacao);
        
        var cacheKey = $"{CacheKeyPrefix}{movimentacao.Id}";
        await _cache.RemoveAsync(cacheKey);
        await _cache.RemoveAsync(CacheKeyAll);
        
        _logger.LogInformation("Cache invalidado após atualização da movimentação {Id}", movimentacao.Id);
        
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        
        if (result)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";
            await _cache.RemoveAsync(cacheKey);
            await _cache.RemoveAsync(CacheKeyAll);
            
            _logger.LogInformation("Cache invalidado após exclusão da movimentação {Id}", id);
        }
        
        return result;
    }
}

// Services/TipoDocumentoService.cs
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using NetRedisASide3.Models;
using NetRedisASide3.Repositories;

namespace NetRedisASide3.Services;

public class TipoDocumentoService
{
    private readonly IRepository<TipoDocumento> _repository;
    private readonly IDistributedCache _cache;
    private readonly ILogger<TipoDocumentoService> _logger;
    private const string CacheKeyPrefix = "tipodocumento:";
    private const string CacheKeyAll = "tiposdocumento:all";
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public TipoDocumentoService(
        IRepository<TipoDocumento> repository,
        IDistributedCache cache,
        ILogger<TipoDocumentoService> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<IEnumerable<TipoDocumento>> GetAllAsync()
    {
        var cachedData = await _cache.GetStringAsync(CacheKeyAll);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Retornando tipos de documento do cache");
            return JsonSerializer.Deserialize<IEnumerable<TipoDocumento>>(cachedData) ?? Enumerable.Empty<TipoDocumento>();
        }

        var tipos = await _repository.GetAllAsync();
        
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = _cacheExpiration
        };
        
        await _cache.SetStringAsync(
            CacheKeyAll,
            JsonSerializer.Serialize(tipos),
            options);

        _logger.LogInformation("Tipos de documento carregados do banco e salvos no cache");
        
        return tipos;
    }

    public async Task<TipoDocumento?> GetByIdAsync(int id)
    {
        var cacheKey = $"{CacheKeyPrefix}{id}";
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (!string.IsNullOrEmpty(cachedData))
        {
            _logger.LogInformation("Retornando tipo de documento {Id} do cache", id);
            return JsonSerializer.Deserialize<TipoDocumento>(cachedData);
        }

        var tipo = await _repository.GetByIdAsync(id);
        
        if (tipo != null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            };
            
            await _cache.SetStringAsync(
                cacheKey,
                JsonSerializer.Serialize(tipo),
                options);

            _logger.LogInformation("Tipo de documento {Id} carregado do banco e salvo no cache", id);
        }
        
        return tipo;
    }

    public async Task<TipoDocumento> AddAsync(TipoDocumento tipo)
    {
        var result = await _repository.AddAsync(tipo);
        
        await _cache.RemoveAsync(CacheKeyAll);
        _logger.LogInformation("Cache de listagem invalidado após criação do tipo de documento {Id}", result.Id);
        
        return result;
    }

    public async Task<TipoDocumento> UpdateAsync(TipoDocumento tipo)
    {
        var result = await _repository.UpdateAsync(tipo);
        
        var cacheKey = $"{CacheKeyPrefix}{tipo.Id}";
        await _cache.RemoveAsync(cacheKey);
        await _cache.RemoveAsync(CacheKeyAll);
        
        _logger.LogInformation("Cache invalidado após atualização do tipo de documento {Id}", tipo.Id);
        
        return result;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var result = await _repository.DeleteAsync(id);
        
        if (result)
        {
            var cacheKey = $"{CacheKeyPrefix}{id}";
            await _cache.RemoveAsync(cacheKey);
            await _cache.RemoveAsync(CacheKeyAll);
            
            _logger.LogInformation("Cache invalidado após exclusão do tipo de documento {Id}", id);
        }
        
        return result;
    }
}