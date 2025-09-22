// ReSharper disable All

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

// https://www.nuget.org/packages/ReprEndpoint
public abstract class ReprResponseEndpoint<TResponse> : ReprEndpointBase
{
    public abstract Task<TResponse> HandleAsync(CancellationToken ct = default);
    protected RouteHandlerBuilder MapPost(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPost(pattern, HandleAsync);
    protected RouteHandlerBuilder MapGet(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapGet(pattern, HandleAsync);
    protected RouteHandlerBuilder MapPut(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPut(pattern, HandleAsync);
    protected RouteHandlerBuilder MapDelete(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapDelete(pattern, HandleAsync);
    protected RouteHandlerBuilder MapPatch(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern) =>
        routes.MapPatch(pattern, HandleAsync);
}