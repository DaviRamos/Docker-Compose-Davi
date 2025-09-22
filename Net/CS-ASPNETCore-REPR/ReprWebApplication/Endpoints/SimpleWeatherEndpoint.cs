// ReSharper disable All

using ReprWebApplication.Endpoints.Models;
using TheReprEndpoint;

namespace ReprWebApplication.Endpoints;

public class SimpleWeatherEndpoint : ReprEndpoint
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        MapGet(routes, "/weatherforecast")
            .WithName("GetSimpleWeatherForecast")
            .WithSummary("Get simple weather forecast - classic endpoint");

        /*
        // Instead of using helper methods
        routes.MapGet("/weatherforecast", HandleAsync)
            .WithName("GetSimpleWeatherForecast")
            .WithSummary("Get simple weather forecast - classic endpoint");
        */
    }

    public override async Task<IResult> HandleAsync(CancellationToken ct = default)
    {
        await Task.Delay(10, ct);

        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray();

        return Results.Ok(forecast);
    }
}