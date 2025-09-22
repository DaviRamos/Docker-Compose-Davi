// ReSharper disable All

using ReprWebApplication.Endpoints.Models;
using TheReprEndpoint;

namespace ReprWebApplication.Endpoints;

public class WeatherStatsEndpoint : ReprResponseEndpoint<WeatherResponse>
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public override string GroupPrefix => "api/weather";

    public override Action<RouteGroupBuilder> ConfigureGroup => group =>
    {
        group.WithTags("Weather Management");
        group.WithOpenApi();
    };

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        // api/weather/stats
        MapGet(routes, "/stats")
            .WithName("GetWeatherStats")
            .WithSummary("Get weather statistics");
    }

    public override async Task<WeatherResponse> HandleAsync(CancellationToken ct = default)
    {
        await Task.Delay(30, ct); // Simulate async work

        // Generate some sample statistics
        var forecasts = Enumerable.Range(1, 7).Select(index =>
                new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
            .ToArray();

        return new WeatherResponse
        {
            Forecasts = forecasts,
            City = "Vienna", // Default city for stats
            Count = forecasts.Length
        };
    }
}