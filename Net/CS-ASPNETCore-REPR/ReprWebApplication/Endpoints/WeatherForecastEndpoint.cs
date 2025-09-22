// ReSharper disable All

using ReprWebApplication.Endpoints.Models;
using TheReprEndpoint;

namespace ReprWebApplication.Endpoints;

public class WeatherForecastEndpoint : ReprEndpoint<GetWeatherRequest, WeatherResponse>
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
    public override bool RequestAsParameters => true;

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        // api/weather/forecast
        MapGet(routes, "/forecast")
            .WithName("GetWeatherForecast")
            .WithSummary("Get weather forecast with parameters");
    }

    public override async Task<WeatherResponse> HandleAsync(GetWeatherRequest request, CancellationToken ct = default)
    {
        await Task.Delay(50, ct); // Simulate async work

        var forecasts = Enumerable.Range(1, request.Days).Select(index =>
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
            City = request.City ?? "Unknown",
            Count = forecasts.Length
        };
    }
}