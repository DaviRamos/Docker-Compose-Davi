// ReSharper disable All

namespace ReprWebApplication.Endpoints.Models;

public class WeatherResponse
{
    public IEnumerable<WeatherForecast> Forecasts { get; set; } = [];
    public string? City { get; set; }
    public int Count { get; set; }
}