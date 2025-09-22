// ReSharper disable All

using MyServices;
using NSubstitute;

namespace TestDoubles;

// Stub: Provides pre-defined answers to calls
public class StubUnitTest
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IEmailService _emailService;
    private readonly OrderService _orderService;

    public StubUnitTest()
    {
        _paymentProcessor = Substitute.For<IPaymentProcessor>();
        _emailService = Substitute.For<IEmailService>();
        _orderService = new OrderService(_paymentProcessor, _emailService);
    }

    [Fact]
    public void ProcessOrder_WithInvalidEmail_ReturnsFalse()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Amount = 100,
            CustomerEmail = "invalid-email"
        };

        // Stub: Configure the email service to return false for email validation
        _emailService.IsEmailValid(order.CustomerEmail).Returns(false);

        // Act
        var result = _orderService.ProcessOrder(order);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void ProcessOrder_WithValidInputs_ReturnsTrue()
    {
        // Arrange
        var order = new Order
        {
            Id = 1,
            Amount = 100,
            CustomerEmail = "valid@example.com"
        };

        // Stubs: Configure return values
        _emailService.IsEmailValid(order.CustomerEmail).Returns(true);
        _paymentProcessor.ProcessPayment(order.Amount).Returns(true);

        // Act
        var result = _orderService.ProcessOrder(order);

        // Assert
        Assert.True(result);
        Assert.True(order.IsPaid);
    }
}

