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
        app.MapGet("/", () => "Hello World!");

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
    }
};