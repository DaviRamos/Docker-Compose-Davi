using ReprWebApplication.Endpoints;
using ReprWebApplication.Endpoints.Models;

namespace ReprWebApplication.UnitTests;

public class SimpleWeatherEndpointTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnOkResultWithWeatherForecasts()
    {
        // Arrange
        var endpoint = new SimpleWeatherEndpoint();

        // Act
        var result = await endpoint.HandleAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.IsType<Microsoft.AspNetCore.Http.HttpResults.Ok<WeatherForecast[]>>(result);
        var okResult = (Microsoft.AspNetCore.Http.HttpResults.Ok<WeatherForecast[]>)result;
        Assert.NotNull(okResult.Value);
        Assert.Equal(5, okResult.Value.Length);
    }
}