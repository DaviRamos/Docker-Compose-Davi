// ReSharper disable All

namespace ReprWebApplication.Endpoints.Models;

public class GetWeatherRequest
{
    public int Days { get; set; } = 5;
    public string? City { get; set; }
}