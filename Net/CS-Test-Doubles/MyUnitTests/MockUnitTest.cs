// ReSharper disable All

using MyServices;
using NSubstitute;

namespace TestDoubles
{
    // Mock: Pre-programmed with expectations of calls they receive
    // They verify behavior, not just state.
    public class MockUnitTest
    {
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IEmailService _emailService;
        private readonly OrderService _orderService;

        public MockUnitTest()
        {
            _paymentProcessor = Substitute.For<IPaymentProcessor>();
            _emailService = Substitute.For<IEmailService>();
            _orderService = new OrderService(_paymentProcessor, _emailService);
        }

        [Fact]
        public void ProcessOrder_WithValidOrder_SendsConfirmationEmail()
        {
            // Arrange
            var order = new Order
            {
                Id = 123,
                Amount = 250,
                CustomerEmail = "customer@example.com"
            };

            // Configure stubs
            _emailService.IsEmailValid(order.CustomerEmail).Returns(true);
            _paymentProcessor.ProcessPayment(order.Amount).Returns(true);

            // Act
            _orderService.ProcessOrder(order);

            // Assert - Mock verification: check that methods were called correctly
            _emailService.Received(1).IsEmailValid(order.CustomerEmail);
            _paymentProcessor.Received(1).ProcessPayment(order.Amount);
            _emailService.Received(1).SendConfirmationEmail(order.CustomerEmail, $"Order {order.Id} confirmed.");
        }

        [Fact]
        public void ProcessOrder_WhenPaymentFails_DoesNotSendEmail()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                Amount = 100,
                CustomerEmail = "test@example.com"
            };

            _emailService.IsEmailValid(order.CustomerEmail).Returns(true);
            _paymentProcessor.ProcessPayment(order.Amount).Returns(false); // Payment fails

            // Act
            var result = _orderService.ProcessOrder(order);

            // Assert
            Assert.False(result);

            // Verify email was NOT sent when payment failed
            _emailService.DidNotReceive().SendConfirmationEmail(Arg.Any<string>(), Arg.Any<string>());
        }
    }
}
