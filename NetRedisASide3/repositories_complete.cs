// Repositories/IRepository.cs
namespace NetRedisASide3.Repositories;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
}

// Repositories/AssuntoRepository.cs
using Microsoft.EntityFrameworkCore;
using NetRedisASide3.Data;
using NetRedisASide3.Models;

namespace NetRedisASide3.Repositories;

public class AssuntoRepository : IRepository<Assunto>
{
    private readonly AppDbContext _context;

    public AssuntoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Assunto>> GetAllAsync()
    {
        return await _context.Assuntos
            .AsNoTracking()
            .OrderByDescending(a => a.DataCriacao)
            .ToListAsync();
    }

    public async Task<Assunto?> GetByIdAsync(int id)
    {
        return await _context.Assuntos
            .AsNoTracking()
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<Assunto> AddAsync(Assunto entity)
    {
        entity.DataCriacao = DateTime.UtcNow;
        entity.DataAtualizacao = DateTime.UtcNow;
        
        _context.Assuntos.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public async Task<Assunto> UpdateAsync(Assunto entity)
    {
        var existingEntity = await _context.Assuntos.FindAsync(entity.Id);
        if (existingEntity == null)
            throw new InvalidOperationException($"Assunto com ID {entity.Id} não encontrado.");

        existingEntity.Nome = entity.Nome;
        existingEntity.Descricao = entity.Descricao;
        existingEntity.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        return existingEntity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Assuntos.FindAsync(id);
        if (entity == null)
            return false;

        _context.Assuntos.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Assuntos.AnyAsync(a => a.Id == id);
    }
}

// Repositories/MovimentacaoRepository.cs
using Microsoft.EntityFrameworkCore;
using NetRedisASide3.Data;
using NetRedisASide3.Models;

namespace NetRedisASide3.Repositories;

public class MovimentacaoRepository : IRepository<Movimentacao>
{
    private readonly AppDbContext _context;

    public MovimentacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Movimentacao>> GetAllAsync()
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .OrderByDescending(m => m.DataCriacao)
            .ToListAsync();
    }

    public async Task<Movimentacao?> GetByIdAsync(int id)
    {
        return await _context.Movimentacoes
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Movimentacao> AddAsync(Movimentacao entity)
    {
        entity.DataCriacao = DateTime.UtcNow;
        entity.DataAtualizacao = DateTime.UtcNow;
        
        _context.Movimentacoes.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public async Task<Movimentacao> UpdateAsync(Movimentacao entity)
    {
        var existingEntity = await _context.Movimentacoes.FindAsync(entity.Id);
        if (existingEntity == null)
            throw new InvalidOperationException($"Movimentação com ID {entity.Id} não encontrada.");

        existingEntity.Nome = entity.Nome;
        existingEntity.Descricao = entity.Descricao;
        existingEntity.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        return existingEntity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.Movimentacoes.FindAsync(id);
        if (entity == null)
            return false;

        _context.Movimentacoes.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Movimentacoes.AnyAsync(m => m.Id == id);
    }
}

// Repositories/TipoDocumentoRepository.cs
using Microsoft.EntityFrameworkCore;
using NetRedisASide3.Data;
using NetRedisASide3.Models;

namespace NetRedisASide3.Repositories;

public class TipoDocumentoRepository : IRepository<TipoDocumento>
{
    private readonly AppDbContext _context;

    public TipoDocumentoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TipoDocumento>> GetAllAsync()
    {
        return await _context.TiposDocumento
            .AsNoTracking()
            .OrderByDescending(t => t.DataCriacao)
            .ToListAsync();
    }

    public async Task<TipoDocumento?> GetByIdAsync(int id)
    {
        return await _context.TiposDocumento
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TipoDocumento> AddAsync(TipoDocumento entity)
    {
        entity.DataCriacao = DateTime.UtcNow;
        entity.DataAtualizacao = DateTime.UtcNow;
        
        _context.TiposDocumento.Add(entity);
        await _context.SaveChangesAsync();
        
        return entity;
    }

    public async Task<TipoDocumento> UpdateAsync(TipoDocumento entity)
    {
        var existingEntity = await _context.TiposDocumento.FindAsync(entity.Id);
        if (existingEntity == null)
            throw new InvalidOperationException($"Tipo de Documento com ID {entity.Id} não encontrado.");

        existingEntity.Nome = entity.Nome;
        existingEntity.Descricao = entity.Descricao;
        existingEntity.DataAtualizacao = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        
        return existingEntity;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var entity = await _context.TiposDocumento.FindAsync(id);
        if (entity == null)
            return false;

        _context.TiposDocumento.Remove(entity);
        await _context.SaveChangesAsync();
        
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.TiposDocumento.AnyAsync(t => t.Id == id);
    }
}