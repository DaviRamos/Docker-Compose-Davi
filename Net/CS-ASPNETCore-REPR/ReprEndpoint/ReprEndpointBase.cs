// ReSharper disable All

using Microsoft.AspNetCore.Routing;

namespace TheReprEndpoint;

// https://www.nuget.org/packages/ReprEndpoint
public abstract class ReprEndpointBase
{
    // https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/route-handlers#route-groups
    // Routing groups in minimal APIs were introduced in ASP.NET Core 7.0 (.NET 7), released in November 2022.
    public virtual string? GroupPrefix => null;
    public virtual Action<RouteGroupBuilder>? ConfigureGroup => null;

    // Built on top of ASP.NET Core Minimal APIs
    public abstract void MapEndpoint(IEndpointRouteBuilder routes);
}