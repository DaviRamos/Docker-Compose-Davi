namespace SOLID;

internal class Program
{
    static void Main(string[] args)
    {
        // BAD EXAMPLE
        var notificationService = new NotificationService();

        notificationService.SendNotification("Hello, this is a test email!", "user@example.com", "email");
        notificationService.SendNotification("Hi, a quick SMS.", "1234567890", "sms");

        var scheduledSender = new ScheduledSmsSender();

        notificationService.SendScheduled(scheduledSender, "This will fail.", "0987654321");



        // GOOD EXAMPLE
        var factory = new NotificationSenderFactory();

        var strategies = new List<INotificationStrategy>
        {
            new EmailNotificationStrategy(factory.CreateEmailSender()),
            new SmsNotificationStrategy(factory.CreateSmsSender()),
            new PushNotificationStrategy(factory.CreatePushNotificationSender()),
            new ScheduledNotificationStrategy(factory.CreateScheduledNotificationSender())
        };

        var notificationService2 = new NotificationService2(strategies);

        notificationService2.SendNotification("Hello!", "user@example.com", NotificationType.Email);
        notificationService2.SendNotification("Hello!", "+1234567890", NotificationType.Sms);
        notificationService2.SendNotification("Hello!", "device123", NotificationType.Push);
        notificationService2.SendNotification("Scheduled Hello!", "user@example.com", NotificationType.Scheduled);
    }
}