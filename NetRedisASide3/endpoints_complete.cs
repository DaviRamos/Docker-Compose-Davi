// Endpoints/AssuntoEndpoints.cs
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NetRedisASide3.Models;
using NetRedisASide3.Services;

namespace NetRedisASide3.Endpoints;

public static class AssuntoEndpoints
{
    public static void MapAssuntoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/assuntos")
            .WithTags("Assuntos")
            .RequireAuthorization();

        group.MapGet("/", GetAll)
            .WithName("GetAllAssuntos")
            .WithOpenApi(op => new(op)
            {
                Summary = "Lista todos os assuntos",
                Description = "Retorna uma lista completa de todos os assuntos cadastrados. Utiliza cache distribuído."
            });

        group.MapGet("/{id:int}", GetById)
            .WithName("GetAssuntoById")
            .WithOpenApi(op => new(op)
            {
                Summary = "Busca assunto por ID",
                Description = "Retorna um assunto específico pelo seu identificador."
            });

        group.MapPost("/", Create)
            .WithName("CreateAssunto")
            .WithOpenApi(op => new(op)
            {
                Summary = "Cria novo assunto",
                Description = "Cria um novo assunto no sistema."
            });

        group.MapPut("/{id:int}", Update)
            .WithName("UpdateAssunto")
            .WithOpenApi(op => new(op)
            {
                Summary = "Atualiza assunto",
                Description = "Atualiza os dados de um assunto existente."
            });

        group.MapDelete("/{id:int}", Delete)
            .WithName("DeleteAssunto")
            .WithOpenApi(op => new(op)
            {
                Summary = "Remove assunto",
                Description = "Remove um assunto do sistema."
            });
    }

    private static async Task<IResult> Create(
        [FromBody] Movimentacao movimentacao,
        MovimentacaoService service,
        IValidator<Movimentacao> validator)
    {
        var validationResult = await validator.ValidateAsync(movimentacao);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var created = await service.AddAsync(movimentacao);
        return Results.Created($"/api/movimentacoes/{created.Id}", created);
    }

    private static async Task<IResult> Update(
        int id,
        [FromBody] Movimentacao movimentacao,
        MovimentacaoService service,
        IValidator<Movimentacao> validator)
    {
        if (id != movimentacao.Id)
        {
            return Results.BadRequest(new { message = "ID da movimentação não corresponde ao ID da URL." });
        }

        var validationResult = await validator.ValidateAsync(movimentacao);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var updated = await service.UpdateAsync(movimentacao);
            return Results.Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static async Task<IResult> Delete(int id, MovimentacaoService service)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound(new { message = $"Movimentação com ID {id} não encontrada." });
    }
}

// Endpoints/TipoDocumentoEndpoints.cs
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NetRedisASide3.Models;
using NetRedisASide3.Services;

namespace NetRedisASide3.Endpoints;

public static class TipoDocumentoEndpoints
{
    public static void MapTipoDocumentoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/tipos-documento")
            .WithTags("Tipos de Documento")
            .RequireAuthorization();

        group.MapGet("/", GetAll)
            .WithName("GetAllTiposDocumento")
            .WithOpenApi(op => new(op)
            {
                Summary = "Lista todos os tipos de documento",
                Description = "Retorna uma lista completa de todos os tipos de documento cadastrados. Utiliza cache distribuído."
            });

        group.MapGet("/{id:int}", GetById)
            .WithName("GetTipoDocumentoById")
            .WithOpenApi(op => new(op)
            {
                Summary = "Busca tipo de documento por ID",
                Description = "Retorna um tipo de documento específico pelo seu identificador."
            });

        group.MapPost("/", Create)
            .WithName("CreateTipoDocumento")
            .WithOpenApi(op => new(op)
            {
                Summary = "Cria novo tipo de documento",
                Description = "Cria um novo tipo de documento no sistema."
            });

        group.MapPut("/{id:int}", Update)
            .WithName("UpdateTipoDocumento")
            .WithOpenApi(op => new(op)
            {
                Summary = "Atualiza tipo de documento",
                Description = "Atualiza os dados de um tipo de documento existente."
            });

        group.MapDelete("/{id:int}", Delete)
            .WithName("DeleteTipoDocumento")
            .WithOpenApi(op => new(op)
            {
                Summary = "Remove tipo de documento",
                Description = "Remove um tipo de documento do sistema."
            });
    }

    private static async Task<IResult> GetAll(TipoDocumentoService service)
    {
        var tipos = await service.GetAllAsync();
        return Results.Ok(tipos);
    }

    private static async Task<IResult> GetById(int id, TipoDocumentoService service)
    {
        var tipo = await service.GetByIdAsync(id);
        return tipo is not null ? Results.Ok(tipo) : Results.NotFound(new { message = $"Tipo de documento com ID {id} não encontrado." });
    }

    private static async Task<IResult> Create(
        [FromBody] TipoDocumento tipo,
        TipoDocumentoService service,
        IValidator<TipoDocumento> validator)
    {
        var validationResult = await validator.ValidateAsync(tipo);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var created = await service.AddAsync(tipo);
        return Results.Created($"/api/tipos-documento/{created.Id}", created);
    }

    private static async Task<IResult> Update(
        int id,
        [FromBody] TipoDocumento tipo,
        TipoDocumentoService service,
        IValidator<TipoDocumento> validator)
    {
        if (id != tipo.Id)
        {
            return Results.BadRequest(new { message = "ID do tipo de documento não corresponde ao ID da URL." });
        }

        var validationResult = await validator.ValidateAsync(tipo);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var updated = await service.UpdateAsync(tipo);
            return Results.Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static async Task<IResult> Delete(int id, TipoDocumentoService service)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound(new { message = $"Tipo de documento com ID {id} não encontrado." });
    }
} GetAll(AssuntoService service)
    {
        var assuntos = await service.GetAllAsync();
        return Results.Ok(assuntos);
    }

    private static async Task<IResult> GetById(int id, AssuntoService service)
    {
        var assunto = await service.GetByIdAsync(id);
        return assunto is not null ? Results.Ok(assunto) : Results.NotFound(new { message = $"Assunto com ID {id} não encontrado." });
    }

    private static async Task<IResult> Create(
        [FromBody] Assunto assunto,
        AssuntoService service,
        IValidator<Assunto> validator)
    {
        var validationResult = await validator.ValidateAsync(assunto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var created = await service.AddAsync(assunto);
        return Results.Created($"/api/assuntos/{created.Id}", created);
    }

    private static async Task<IResult> Update(
        int id,
        [FromBody] Assunto assunto,
        AssuntoService service,
        IValidator<Assunto> validator)
    {
        if (id != assunto.Id)
        {
            return Results.BadRequest(new { message = "ID do assunto não corresponde ao ID da URL." });
        }

        var validationResult = await validator.ValidateAsync(assunto);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var updated = await service.UpdateAsync(assunto);
            return Results.Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound(new { message = ex.Message });
        }
    }

    private static async Task<IResult> Delete(int id, AssuntoService service)
    {
        var deleted = await service.DeleteAsync(id);
        return deleted ? Results.NoContent() : Results.NotFound(new { message = $"Assunto com ID {id} não encontrado." });
    }
}

// Endpoints/MovimentacaoEndpoints.cs
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using NetRedisASide3.Models;
using NetRedisASide3.Services;

namespace NetRedisASide3.Endpoints;

public static class MovimentacaoEndpoints
{
    public static void MapMovimentacaoEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/movimentacoes")
            .WithTags("Movimentações")
            .RequireAuthorization();

        group.MapGet("/", GetAll)
            .WithName("GetAllMovimentacoes")
            .WithOpenApi(op => new(op)
            {
                Summary = "Lista todas as movimentações",
                Description = "Retorna uma lista completa de todas as movimentações cadastradas. Utiliza cache distribuído."
            });

        group.MapGet("/{id:int}", GetById)
            .WithName("GetMovimentacaoById")
            .WithOpenApi(op => new(op)
            {
                Summary = "Busca movimentação por ID",
                Description = "Retorna uma movimentação específica pelo seu identificador."
            });

        group.MapPost("/", Create)
            .WithName("CreateMovimentacao")
            .WithOpenApi(op => new(op)
            {
                Summary = "Cria nova movimentação",
                Description = "Cria uma nova movimentação no sistema."
            });

        group.MapPut("/{id:int}", Update)
            .WithName("UpdateMovimentacao")
            .WithOpenApi(op => new(op)
            {
                Summary = "Atualiza movimentação",
                Description = "Atualiza os dados de uma movimentação existente."
            });

        group.MapDelete("/{id:int}", Delete)
            .WithName("DeleteMovimentacao")
            .WithOpenApi(op => new(op)
            {
                Summary = "Remove movimentação",
                Description = "Remove uma movimentação do sistema."
            });
    }

    private static async Task<IResult> GetAll(MovimentacaoService service)
    {
        var movimentacoes = await service.GetAllAsync();
        return Results.Ok(movimentacoes);
    }

    private static async Task<IResult> GetById(int id, MovimentacaoService service)
    {
        var movimentacao = await service.GetByIdAsync(id);
        return movimentacao is not null ? Results.Ok(movimentacao) : Results.NotFound(new { message = $"Movimentação com ID {id} não encontrada." });
    }

    private static async Task<IResult>