using NetRedisASide.Data;
using NetRedisASide.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace NetRedisASide.Endpoints;

public static class MovimentacaoEndpoints
{
    public static void MapMovimentacaoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/movimentacoes", async (
            Movimentacao movimentacao,
            AppDbContext db,
            IDistributedCache cache) =>
        {
            movimentacao.DataCriacao = DateTime.Now;
            db.Movimentacoes.Add(movimentacao);

            await db.SaveChangesAsync();
            await cache.RemoveAsync("movimentacoesCache");
            return Results.Created($"/movimentacoes/{movimentacao.Id}", movimentacao);
        });

        app.MapGet("/movimentacoes", async (
            AppDbContext db,
            IDistributedCache cache) =>
        {
            var cacheKey = "movimentacoesCache";
            var cachedMovimentacoes = await cache.GetStringAsync(cacheKey);

            if (cachedMovimentacoes is not null)
            {
                var movimentacoesFromCache = JsonSerializer.Deserialize<List<Movimentacao>>(cachedMovimentacoes);
                return Results.Ok(movimentacoesFromCache);
            }

            var movimentacoes = await db.Movimentacoes.AsNoTracking().ToListAsync();
            var serializedMovimentacoes = JsonSerializer.Serialize(movimentacoes);
            await cache.SetStringAsync(cacheKey, serializedMovimentacoes);

            return Results.Ok(movimentacoes);
        });

        app.MapGet("/movimentacoes/{id:int}", async (
            int id,
            AppDbContext db,
            IDistributedCache cache) =>
        {
            var cacheKey = $"movimentacao-{id}";
            var cachedMovimentacao = await cache.GetStringAsync(cacheKey);

            if (cachedMovimentacao is not null)
            {
                var movimentacaoFromCache = JsonSerializer.Deserialize<Movimentacao>(cachedMovimentacao);
                return Results.Ok(movimentacaoFromCache);
            }

            var movimentacao = await db.Movimentacoes.AsNoTracking().FirstOrDefaultAsync(m => m.Id == id);
            if (movimentacao is null)
            {
                return Results.NotFound();
            }

            var serializedMovimentacao = JsonSerializer.Serialize(movimentacao);
            await cache.SetStringAsync(cacheKey, serializedMovimentacao);

            return Results.Ok(movimentacao);
        });

        app.MapPut("/movimentacoes/{id:int}", async (
            int id,
            Movimentacao updatedMovimentacao,
            AppDbContext db,
            IDistributedCache cache) =>
        {
            var movimentacao = await db.Movimentacoes.FirstOrDefaultAsync(m => m.Id == id);
            if (movimentacao is null)
            {
                return Results.NotFound();
            }

            movimentacao.Descricao = updatedMovimentacao.Descricao;
            movimentacao.Nome = updatedMovimentacao.Nome;
            movimentacao.DataAtualizacao = DateTime.Now;

            await db.SaveChangesAsync();
            await cache.RemoveAsync("movimentacoesCache");
            await cache.RemoveAsync($"movimentacao-{id}");

            return Results.Ok(movimentacao);
        });

        app.MapDelete("/movimentacoes/{id:int}", async (
           int id,
           AppDbContext db,
           IDistributedCache cache) =>
       {
           var movimentacao = await db.Movimentacoes.FirstOrDefaultAsync(m => m.Id == id);
           if (movimentacao is null)
           {
               return Results.NotFound();
           }

           db.Movimentacoes.Remove(movimentacao);
           await db.SaveChangesAsync();
           await cache.RemoveAsync("movimentacoesCache");
           await cache.RemoveAsync($"movimentacao-{id}");

           return Results.NoContent();
       });
    }
}