using NetRedisASide.Data;
using NetRedisASide.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace NetRedisASide.Endpoints;

public static class AssuntoEndpoints
{
    public static void MapAssuntoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/", () => "Hello World!");

        app.MapPost("v1/Assuntos", async (

            Assunto assunto,
            AppDbContext dbContext,
            IDistributedCache cache) =>
        {
            dbContext.Assuntos.Add(assunto);
            await dbContext.SaveChangesAsync();

            // Invalidate the cache
            await cache.RemoveAsync($"Assunto-{assunto.Id}");
        });

        app.MapGet("v1/assuntos", async (
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var cacheKey = "AssuntosList";
            var cachedAssuntos = await cache.GetStringAsync(cacheKey);

            if (cachedAssuntos is not null)
            {
                return Results.Ok(cachedAssuntos);
            }

            var assuntos = await context.Assuntos.AsNoTracking().ToListAsync();
            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(assuntos));

            return Results.Ok(assuntos);
        });


        app.MapGet("v1/Assuntos/{id:int}", async (
            int id,
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var cacheKey = $"Assunto-{id}";
            var cachedAssunto = await cache.GetStringAsync(cacheKey);

            if (cachedAssunto is not null)
            {
                var assuntoFromCache = JsonSerializer.Deserialize<Assunto>(cachedAssunto);
                return Results.Ok(assuntoFromCache);
            }

            var assunto = await context.Assuntos.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            if (assunto is null)
            {
                return Results.NotFound();
            }

            await cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(assunto));
            return Results.Ok(assunto);
        });

        app.MapPut("v1/Assuntos/{id:int}", async (
            int id,
            Assunto updatedAssunto,
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var assunto = await context.Assuntos.FindAsync(id);
            if (assunto is null)
            {
                return Results.NotFound();
            }

            assunto.Nome = updatedAssunto.Nome;
            await context.SaveChangesAsync();

            // Invalidate the cache
            await cache.RemoveAsync($"Assunto-{id}");
            await cache.RemoveAsync("AssuntosList");

            return Results.NoContent();
        });

        app.MapDelete("v1/Assuntos/{id:int}", async (
            int id,
            AppDbContext context,
            IDistributedCache cache) =>
        {
            var assunto = await context.Assuntos.FindAsync(id);
            if (assunto is null)
            {
                return Results.NotFound();
            }

            context.Assuntos.Remove(assunto);
            await context.SaveChangesAsync();

            // Invalidate the cache
            await cache.RemoveAsync($"Assunto-{id}");
            await cache.RemoveAsync("AssuntosList");

            return Results.NoContent();
        });
    }
};