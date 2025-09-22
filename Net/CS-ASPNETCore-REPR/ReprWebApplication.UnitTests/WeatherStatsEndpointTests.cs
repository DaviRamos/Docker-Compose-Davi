using ReprWebApplication.Endpoints;

namespace ReprWebApplication.UnitTests;

public class WeatherStatsEndpointTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnWeatherResponseWith7Days()
    {
        // Arrange
        var endpoint = new WeatherStatsEndpoint();

        // Act
        var result = await endpoint.HandleAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Vienna", result.City);
        Assert.Equal(7, result.Count);
    }
}