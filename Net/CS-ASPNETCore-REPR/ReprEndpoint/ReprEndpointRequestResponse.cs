// ReSharper disable All

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

// https://www.nuget.org/packages/ReprEndpoint
public abstract class ReprEndpoint<TRequest, TResponse> : ReprEndpointBase
    where TRequest : notnull // Ensures that TRequest is a non-nullable type, which is useful for AsParameters attribute.
{
    /*
     * AsParameters is an attribute in ASP.NET Core that allows you to bind multiple parameters from a single object's properties
     * to action method parameters. Instead of defining each parameter individually, you can create a class with properties
     * and mark it with [AsParameters] to automatically map those properties to route values, query strings, headers, or request body.
     * [AsParameters] was introduced in ASP.NET Core 7.0 (.NET 7), released in November 2022.
     */
    public virtual bool RequestAsParameters => false;
    public abstract Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
    protected RouteHandlerBuilder MapPost(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPost(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPost(pattern, HandleAsync);
    }
    protected RouteHandlerBuilder MapGet(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapGet(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapGet(pattern, HandleAsync);
    }
    protected RouteHandlerBuilder MapPut(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPut(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPut(pattern, HandleAsync);
    }
    protected RouteHandlerBuilder MapDelete(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapDelete(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapDelete(pattern, HandleAsync);
    }
    protected RouteHandlerBuilder MapPatch(IEndpointRouteBuilder routes, [StringSyntax("Route")] string pattern)
    {
        return RequestAsParameters
            ? routes.MapPatch(pattern, ([AsParameters] TRequest request, CancellationToken ct) => HandleAsync(request, ct))
            : routes.MapPatch(pattern, HandleAsync);
    }
}