// ReSharper disable All

using MyServices;
using NSubstitute;

namespace TestDoubles
{
    public class SpyUnitTest
    {
        private readonly IPaymentProcessor _paymentProcessor;
        private readonly IEmailService _emailService;
        private readonly OrderService _orderService;

        public SpyUnitTest()
        {
            _paymentProcessor = Substitute.For<IPaymentProcessor>();
            _emailService = Substitute.For<IEmailService>();
            _orderService = new OrderService(_paymentProcessor, _emailService);
        }

        [Fact]
        public void ProcessOrder_CallsMethodsInCorrectOrder()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                Amount = 100,
                CustomerEmail = "test@example.com"
            };

            _emailService.IsEmailValid(Arg.Any<string>()).Returns(true);
            _paymentProcessor.ProcessPayment(Arg.Any<decimal>()).Returns(true);

            // Act
            _orderService.ProcessOrder(order);

            // Assert - Spy behavior: verify call order
            Received.InOrder(() =>
            {
                _emailService.IsEmailValid(order.CustomerEmail);
                _paymentProcessor.ProcessPayment(order.Amount);
                _emailService.SendConfirmationEmail(Arg.Any<string>(), Arg.Any<string>());
            });
        }

        [Fact]
        public void ProcessOrder_RecordsAllArguments()
        {
            // Arrange
            var order = new Order
            {
                Id = 456,
                Amount = 75.50m,
                CustomerEmail = "specific@test.com"
            };

            _emailService.IsEmailValid(Arg.Any<string>()).Returns(true);
            _paymentProcessor.ProcessPayment(Arg.Any<decimal>()).Returns(true);

            // Act
            _orderService.ProcessOrder(order);

            // Assert - Verify exact arguments were passed
            _emailService.Received().IsEmailValid(order.CustomerEmail);
            _paymentProcessor.Received().ProcessPayment(order.Amount);
            _emailService.Received().SendConfirmationEmail(order.CustomerEmail, $"Order {order.Id} confirmed.");
        }
    }
}
