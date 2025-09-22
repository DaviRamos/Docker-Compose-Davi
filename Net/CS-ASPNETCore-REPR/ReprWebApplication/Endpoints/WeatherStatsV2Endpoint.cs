// ReSharper disable All

using Asp.Versioning;
using ReprWebApplication.Endpoints.Models;
using TheReprEndpoint;

namespace ReprWebApplication.Endpoints;

public class WeatherStatsV2Endpoint : ReprResponseEndpoint<WeatherResponse>
{
    private static readonly string[] Summaries =
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherStatsV2Endpoint> _logger;

    public WeatherStatsV2Endpoint(ILogger<WeatherStatsV2Endpoint> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override string GroupPrefix => "api/weather";

    public override Action<RouteGroupBuilder> ConfigureGroup => group =>
    {
        group.WithTags("Weather Management");
        group.WithOpenApi();
    };

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        var versionSet = routes.NewApiVersionSet()
            .HasApiVersion(new ApiVersion(2, 0))
            .Build();

        // api/weather/v2/stats
        MapGet(routes, "/v{version:apiVersion}/stats")
            .WithName("GetWeatherStatsV2")
            .WithApiVersionSet(versionSet)
            .WithOpenApi()
            .WithSummary("Get weather statistics");
    }

    public override async Task<WeatherResponse> HandleAsync(CancellationToken ct = default)
    {
        _logger.LogInformation("Generating weather forecast for 7 days (V2)");

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

        _logger.LogDebug("Generated {ForecastCount} weather stats (V2)", forecasts.Length);

        return new WeatherResponse
        {
            Forecasts = forecasts,
            City = "Vienna", // Default city for stats
            Count = forecasts.Length
        };
    }
}