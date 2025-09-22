// ReSharper disable All

namespace MyServices;

public class Order
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public bool IsPaid { get; set; }
}

// Abstraction for different payment gateways (Stripe, PayPal, etc.)
public interface IPaymentProcessor
{
    bool ProcessPayment(decimal amount);
}

// Abstraction for email operations to decouple from specific email providers (SMTP, SendGrid, etc.)
public interface IEmailService
{
    void SendConfirmationEmail(string email, string message);
    bool IsEmailValid(string email);
}

// Business logic coordinator that manages the complete order fulfillment process
public class OrderService
{
    private readonly IPaymentProcessor _paymentProcessor;
    private readonly IEmailService _emailService;

    public OrderService(IPaymentProcessor paymentProcessor, IEmailService emailService)
    {
        _paymentProcessor = paymentProcessor;
        _emailService = emailService;
    }

    /// <summary>
    /// Processes a customer order through the complete payment and notification workflow.
    /// 
    /// Scenario: A customer has placed an order and this method handles the end-to-end processing:
    /// 1. Validates the order amount is positive (rejects $0 or negative amounts)
    /// 2. Confirms the customer's email address is properly formatted
    /// 3. Attempts to charge the customer's payment method for the order amount
    /// 4. If payment succeeds, marks the order as paid and sends a confirmation email
    /// </summary>
    /// <param name="order">The order containing amount, customer email, and order details</param>
    /// <returns>True if the entire process succeeds; False if any validation or payment step fails</returns>
    public bool ProcessOrder(Order order)
    {
        if (order.Amount <= 0)
            return false;

        if (!_emailService.IsEmailValid(order.CustomerEmail))
            return false;

        if (!_paymentProcessor.ProcessPayment(order.Amount))
            return false;

        order.IsPaid = true;
        _emailService.SendConfirmationEmail(order.CustomerEmail, $"Order {order.Id} confirmed.");

        return true;
    }
}