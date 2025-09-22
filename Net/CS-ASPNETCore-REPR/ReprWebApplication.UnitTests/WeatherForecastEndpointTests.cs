using ReprWebApplication.Endpoints;
using ReprWebApplication.Endpoints.Models;

namespace ReprWebApplication.UnitTests;

public class WeatherForecastEndpointTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnWeatherResponseWithCorrectDays()
    {
        // Arrange
        var endpoint = new WeatherForecastEndpoint();
        var request = new GetWeatherRequest { Days = 3, City = "Vienna" };

        // Act
        var result = await endpoint.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Vienna", result.City);
        Assert.Equal(3, result.Count);
    }
}