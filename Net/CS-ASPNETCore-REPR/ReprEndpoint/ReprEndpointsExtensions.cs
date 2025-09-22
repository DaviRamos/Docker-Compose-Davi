// ReSharper disable All

using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace TheReprEndpoint;

// https://www.nuget.org/packages/ReprEndpoint
public static class ReprEndpointsExtensions
{
    public static IServiceCollection AddReprEndpoints(this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
        params Assembly[]? assemblies)
    {
        var assembliesToScan = assemblies?.Length > 0 ? assemblies : AppDomain.CurrentDomain.GetAssemblies();
        var endpointTypes = assembliesToScan
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsSubclassOf(typeof(ReprEndpointBase)) && !type.IsAbstract);
        foreach (var type in endpointTypes)
        {
            services.Add(new ServiceDescriptor(type, type, serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(ReprEndpointBase), type, serviceLifetime));
        }
        return services;
    }
    public static IServiceCollection AddReprEndpoints(this IServiceCollection services,
        ServiceLifetime serviceLifetime,
        params Type[]? endpointTypes)
    {
        if (endpointTypes == null || endpointTypes.Length == 0)
            return services;
        foreach (var type in endpointTypes)
        {
            if (!type.IsSubclassOf(typeof(ReprEndpointBase)))
                throw new ArgumentException($"Type {type.Name} must inherit from {nameof(ReprEndpointBase)}", nameof(endpointTypes));
            if (type.IsAbstract)
                throw new ArgumentException($"Type {type.Name} cannot be abstract", nameof(endpointTypes));
        }
        foreach (var type in endpointTypes)
        {
            services.Add(new ServiceDescriptor(type, type, serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(ReprEndpointBase), type, serviceLifetime));
        }
        return services;
    }
    public static WebApplication MapReprEndpoints(this WebApplication app)
    {
        var endpoints = app.Services.GetServices<ReprEndpointBase>();
        foreach (var endpoint in endpoints)
        {
            if (!string.IsNullOrWhiteSpace(endpoint.GroupPrefix))
            {
                var group = app.MapGroup(endpoint.GroupPrefix);
                endpoint.ConfigureGroup?.Invoke(group);
                endpoint.MapEndpoint(group);
            }
            else
            {
                endpoint.MapEndpoint(app);
            }
        }
        return app;
    }
}