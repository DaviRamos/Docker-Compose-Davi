using Microsoft.Extensions.Logging;
using NSubstitute;
using ReprWebApplication.Endpoints;

namespace ReprWebApplication.UnitTests;

public class WeatherStatsV2EndpointTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnWeatherResponseAndLog()
    {
        // Arrange
        var logger = Substitute.For<ILogger<WeatherStatsV2Endpoint>>();
        var endpoint = new WeatherStatsV2Endpoint(logger);

        // Act
        var result = await endpoint.HandleAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Vienna", result.City);
        Assert.Equal(7, result.Count);
        logger.Received(1).LogInformation("Generating weather forecast for 7 days (V2)");
    }
}