// ReSharper disable All

using MyServices;
using NSubstitute;

namespace TestDoubles
{
    // In-memory implementation of IEmailService for testing purposes
    public class FakeEmailService : IEmailService
    {
        public List<string> SentEmails { get; } = new();
        public bool EmailValidationResult { get; set; } = true;

        public void SendConfirmationEmail(string email, string message)
        {
            SentEmails.Add($"To: {email}, Message: {message}");
        }

        public bool IsEmailValid(string email)
        {
            return EmailValidationResult && email.Contains("@") && email.Contains(".");
        }
    }

    // Fake: Working implementations with shortcuts (like in-memory database)
    public class FakeUnitTest
    {
        [Fact]
        public void ProcessOrder_WithFakeEmailService_RecordsEmailsSent()
        {
            // Arrange
            var paymentProcessor = Substitute.For<IPaymentProcessor>();

            var fakeEmailService = new FakeEmailService();
            var orderService = new OrderService(paymentProcessor, fakeEmailService);

            var order = new Order
            {
                Id = 1,
                Amount = 100,
                CustomerEmail = "fake@test.com"
            };

            paymentProcessor.ProcessPayment(order.Amount).Returns(true);

            // Act
            orderService.ProcessOrder(order);

            // Assert
            Assert.Single(fakeEmailService.SentEmails);
            Assert.Contains(order.CustomerEmail, fakeEmailService.SentEmails[0]);
        }
    }
}
