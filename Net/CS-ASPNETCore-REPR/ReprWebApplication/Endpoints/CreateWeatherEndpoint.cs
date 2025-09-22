// ReSharper disable All

using ReprWebApplication.Endpoints.Models;
using TheReprEndpoint;

namespace ReprWebApplication.Endpoints;

public class CreateWeatherEndpoint : ReprRequestEndpoint<CreateWeatherRequest>
{
    private static readonly List<WeatherForecast> WeatherDataStore = new();

    public override string GroupPrefix => "api/weather";

    public override Action<RouteGroupBuilder> ConfigureGroup => group =>
    {
        group.WithTags("Weather Management");
        group.WithOpenApi();
    };

    public override void MapEndpoint(IEndpointRouteBuilder routes)
    {
        // api/weather/create
        MapPost(routes, "/create")
            .WithName("CreateWeatherForecast")
            .WithSummary("Create a new weather forecast entry");
    }

    public override async Task<IResult> HandleAsync(CreateWeatherRequest request, CancellationToken ct = default)
    {
        await Task.Delay(25, ct); // Simulate async work

        var weatherForecast = new WeatherForecast
        {
            Date = request.Date,
            TemperatureC = request.TemperatureC,
            Summary = request.Summary
        };

        WeatherDataStore.Add(weatherForecast);

        return Results.Created($"/api/weather/create", new
        {
            Message = $"Weather forecast created for {request.City ?? "Unknown city"}",
            Data = weatherForecast
        });
    }
}