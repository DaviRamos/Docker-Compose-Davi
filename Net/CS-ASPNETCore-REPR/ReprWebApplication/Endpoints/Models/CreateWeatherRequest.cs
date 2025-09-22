// ReSharper disable All

namespace ReprWebApplication.Endpoints.Models;

public class CreateWeatherRequest
{
    public DateOnly Date { get; set; }
    public int TemperatureC { get; set; }
    public string? Summary { get; set; }
    public string? City { get; set; }
}