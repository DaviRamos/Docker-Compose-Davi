// ReSharper disable All

using MyServices;
using NSubstitute;

namespace MyUnitTests
{
    // Dummy: Objects passed around but never actually used
    public class DummyUnitTest
    {
        [Fact]
        public void ProcessOrder_WithZeroAmount_ReturnsFalse()
        {
            // Arrange
            var order = new Order
            {
                Id = 1,
                Amount = 0,
                CustomerEmail = "test@example.com"
            };

            // We create dummy objects, but they won't be called
            // because the method returns early due to zero amount
            var dummyPaymentProcessor = Substitute.For<IPaymentProcessor>();
            var dummyEmailService = Substitute.For<IEmailService>();
            var orderService = new OrderService(dummyPaymentProcessor, dummyEmailService);

            // Act
            var result = orderService.ProcessOrder(order);

            // Assert
            Assert.False(result);

            // Verify our dummies were never called
            dummyPaymentProcessor.DidNotReceive().ProcessPayment(Arg.Any<decimal>());
            dummyEmailService.DidNotReceive().IsEmailValid(Arg.Any<string>());
        }
    }
}
