using Microsoft.AspNetCore.Http;
using ReprWebApplication.Endpoints;
using ReprWebApplication.Endpoints.Models;

namespace ReprWebApplication.UnitTests;

public class CreateWeatherEndpointTests
{
    [Fact]
    public async Task HandleAsync_ShouldReturnCreatedResult()
    {
        // Arrange
        var endpoint = new CreateWeatherEndpoint();
        var request = new CreateWeatherRequest
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
            TemperatureC = 25,
            Summary = "Sunny",
            City = "Vienna"
        };

        // Act
        var result = await endpoint.HandleAsync(request, TestContext.Current.CancellationToken);

        // Assert
        Assert.IsAssignableFrom<IResult>(result);
        var resultType = result.GetType();
        Assert.Equal("Created`1", resultType.Name);
    }
}