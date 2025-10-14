using NetRedisASide.Data;
using NetRedisASide.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace NetRedisASide.Endpoints;

public static class TipoDocumentoEndpoints
{
    public static void MapTipoDocumentoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("v1/TiposDocumentos", async (

            TipoDocumento tipoDocumento,
            AppDbContext dbContext,
            IDistributedCache cache) =>
        {
            dbContext.TiposDocumentos.Add(tipoDocumento);
            await dbContext.SaveChangesAsync();

            // Invalidate the cache
            await cache.RemoveAsync($"TipoDocumento-{tipoDocumento.Id}");
        });

        app.MapGet("v1/TiposDocumentoss", async (
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var cacheKey = "TipoDocumentosList";
            var cachedTipoDocumentos = await cache.GetStringAsync(cacheKey);

            if (cachedTipoDocumentos is not null)
            {
                return Results.Ok(cachedTipoDocumentos);
            }

            var tipoDocumentos = await context.TiposDocumentos.AsNoTracking().ToListAsync();
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(tipoDocumentos));

            return Results.Ok(tipoDocumentos);
        });


        app.MapGet("v1/TiposDocumentos/{id:int}", async (
            int id,
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var cacheKey = $"TipoDocumento-{id}";
            var cachedTipoDocumento = await cache.GetStringAsync(cacheKey);

            if (cachedTipoDocumento is not null)
            {
                var tipoDocumentoFromCache = JsonSerializer.Deserialize<TipoDocumento>(cachedTipoDocumento);
                return Results.Ok(tipoDocumentoFromCache);
            }

            var tipoDocumento = await context.TiposDocumentos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (tipoDocumento is null)
            {
                return Results.NotFound();
            }

            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(tipoDocumento));
            return Results.Ok(tipoDocumento);
        });

        app.MapPut("v1/TiposDocumentos/{id:int}", async (
            int id,
            TipoDocumento updatedTipoDocumento,
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var tipoDocumento = await context.TiposDocumentos.FindAsync(id);
            if (tipoDocumento is null)
            {
                return Results.NotFound();
            }

            tipoDocumento.Nome = updatedTipoDocumento.Nome;
            tipoDocumento.Descricao = updatedTipoDocumento.Descricao;
            tipoDocumento.DataAtualizacao = DateTime.Now;
            await context.SaveChangesAsync();

            // Invalidate the cache
            await cache.RemoveAsync($"TipoDocumento-{id}");
            await cache.RemoveAsync("TiposDocumentosList");

            return Results.NoContent();
        });

        app.MapDelete("v1/TiposDocumentos/{id:int}", async (
            int id,
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var tipoDocumento = await context.TiposDocumentos.FindAsync(id);
            if (tipoDocumento is null)
            {
                return Results.NotFound();
            }

            context.TiposDocumentos.Remove(tipoDocumento);
            await context.SaveChangesAsync();

            // Invalidate the cache
            await cache.RemoveAsync($"TipoDocumento-{id}");
            await cache.RemoveAsync("TiposDocumentosList");

            return Results.NoContent();
        });
    }
};