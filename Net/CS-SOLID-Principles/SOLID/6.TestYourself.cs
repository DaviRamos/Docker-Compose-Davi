// ReSharper disable ALL

namespace SOLID;

// BAD EXAMPLES
// Interface Segregation Principle (ISP) Violation
public interface INotificationSender
{
    void SendEmail(string recipient, string message);
    void SendSms(string phoneNumber, string message);
    void SendPushNotification(string deviceId, string message);
    void SendScheduledNotification(string recipient, string message, DateTime scheduleTime);
}

// Single Responsibility Principle (SRP) Violation
// 1. Routing notifications based on type
// 2. Managing different sender instances
// 3. Handling scheduling logic
public class NotificationService
{
    // Dependency Inversion Principle (DIP) Violation
    private readonly EmailSender _emailSender = new EmailSender();
    private readonly SmsSender _smsSender = new SmsSender();

    public void SendNotification(string message, string recipient, string type)
    {
        // Open/Closed Principle (OCP) Violation:
        if (string.Equals(type, "email", StringComparison.OrdinalIgnoreCase))
        {
            _emailSender.SendEmail(recipient, message);
        }
        else if (string.Equals(type, "sms", StringComparison.OrdinalIgnoreCase))
        {
            _smsSender.SendSms(recipient, message);
        }
    }

    // Dependency Inversion Principle (DIP) Violation
    public void SendScheduled(SmsSender sender, string message, string recipient)
    {
        sender.ScheduleSms(recipient, message, DateTime.Now.AddHours(1));
    }
}

public class SmsSender
{
    public virtual void SendSms(string phoneNumber, string message)
    {
        Console.WriteLine($"Sending SMS to {phoneNumber}: {message}");
    }

    public virtual void ScheduleSms(string phoneNumber, string message, DateTime time)
    {
        Console.WriteLine($"Scheduling SMS to {phoneNumber} for {time}: {message}");
    }
}

// Liskov Substitution Principle (LSP) Violation
public class ScheduledSmsSender : SmsSender
{
    public override void SendSms(string phoneNumber, string message)
    {
        throw new NotImplementedException("This sender can only schedule messages.");
    }

    public override void ScheduleSms(string phoneNumber, string message, DateTime time)
    {
        Console.WriteLine($"[ScheduledSmsSender] Scheduling SMS to {phoneNumber} for {time}: {message}");
    }
}

public class EmailSender
{
    public void SendEmail(string recipient, string message)
    {
        Console.WriteLine($"Sending Email to {recipient}: {message}");
    }
}

// Interface Segregation Principle (ISP) Violation
public class PushNotificationSender : INotificationSender
{
    public void SendPushNotification(string deviceId, string message)
    {
        Console.WriteLine($"Sending Push Notification to {deviceId}: {message}");
    }

    public void SendEmail(string recipient, string message)
    {
        throw new NotImplementedException("This class only handles push notifications.");
    }

    public void SendSms(string phoneNumber, string message)
    {
        throw new NotImplementedException("This class only handles push notifications.");
    }

    public void SendScheduledNotification(string recipient, string message, DateTime scheduleTime)
    {
        throw new NotImplementedException("This class only handles push notifications.");
    }
}

// GOOD EXAMPLES
public interface IEmailSender
{
    void SendEmail(string recipient, string message);
}

public interface ISmsSender
{
    void SendSms(string phoneNumber, string message);
}

public interface IPushNotificationSender
{
    void SendPushNotification(string deviceId, string message);
}

public interface IScheduledNotificationSender
{
    void SendScheduledNotification(string recipient, string message, DateTime scheduleTime);
}

public interface INotificationSenderFactory
{
    IEmailSender CreateEmailSender();
    ISmsSender CreateSmsSender();
    IPushNotificationSender CreatePushNotificationSender();
    IScheduledNotificationSender CreateScheduledNotificationSender();
}

public enum NotificationType
{
    Email,
    Sms,
    Push,
    Scheduled
}

public interface INotificationStrategy
{
    void SendNotification(string recipient, string message);
    bool CanHandle(NotificationType type);
}

public class EmailSender2 : IEmailSender
{
    public void SendEmail(string recipient, string message)
    {
        Console.WriteLine($"Sending Email to {recipient}: {message}");
    }
}

public class SmsSender2 : ISmsSender
{
    public virtual void SendSms(string phoneNumber, string message)
    {
        Console.WriteLine($"Sending SMS to {phoneNumber}: {message}");
    }
}

public class PushNotificationSender2 : IPushNotificationSender
{
    public void SendPushNotification(string deviceId, string message)
    {
        Console.WriteLine($"Sending Push Notification to {deviceId}: {message}");
    }
}

public class ScheduledNotificationSender2 : IScheduledNotificationSender
{
    public void SendScheduledNotification(string recipient, string message, DateTime scheduleTime)
    {
        Console.WriteLine($"Scheduling notification to {recipient} for {scheduleTime}: {message}");
    }
}

public class EmailNotificationStrategy : INotificationStrategy
{
    private readonly IEmailSender _emailSender;

    public EmailNotificationStrategy(IEmailSender emailSender)
    {
        _emailSender = emailSender ?? throw new ArgumentNullException(nameof(emailSender));
    }

    public void SendNotification(string recipient, string message)
    {
        _emailSender.SendEmail(recipient, message);
    }

    public bool CanHandle(NotificationType type)
    {
        return type == NotificationType.Email;
    }
}

public class SmsNotificationStrategy : INotificationStrategy
{
    private readonly ISmsSender _smsSender;

    public SmsNotificationStrategy(ISmsSender smsSender)
    {
        _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
    }

    public void SendNotification(string recipient, string message)
    {
        _smsSender.SendSms(recipient, message);
    }

    public bool CanHandle(NotificationType type)
    {
        return type == NotificationType.Sms;
    }
}

public class PushNotificationStrategy : INotificationStrategy
{
    private readonly IPushNotificationSender _pushSender;

    public PushNotificationStrategy(IPushNotificationSender pushSender)
    {
        _pushSender = pushSender ?? throw new ArgumentNullException(nameof(pushSender));
    }

    public void SendNotification(string recipient, string message)
    {
        _pushSender.SendPushNotification(recipient, message);
    }

    public bool CanHandle(NotificationType type)
    {
        return type == NotificationType.Push;
    }
}

public class ScheduledNotificationStrategy : INotificationStrategy
{
    private readonly IScheduledNotificationSender _scheduledSender;

    public ScheduledNotificationStrategy(IScheduledNotificationSender scheduledSender)
    {
        _scheduledSender = scheduledSender ?? throw new ArgumentNullException(nameof(scheduledSender));
    }

    public void SendNotification(string recipient, string message)
    {
        _scheduledSender.SendScheduledNotification(recipient, message, DateTime.Now.AddHours(1));
    }

    public bool CanHandle(NotificationType type)
    {
        return type == NotificationType.Scheduled;
    }
}

public class NotificationSenderFactory : INotificationSenderFactory
{
    public IEmailSender CreateEmailSender()
    {
        return new EmailSender2();
    }

    public ISmsSender CreateSmsSender()
    {
        return new SmsSender2();
    }

    public IPushNotificationSender CreatePushNotificationSender()
    {
        return new PushNotificationSender2();
    }

    public IScheduledNotificationSender CreateScheduledNotificationSender()
    {
        return new ScheduledNotificationSender2();
    }
}

public class NotificationService2
{
    private readonly IEnumerable<INotificationStrategy> _strategies;

    public NotificationService2(IEnumerable<INotificationStrategy> strategies)
    {
        _strategies = strategies ?? throw new ArgumentNullException(nameof(strategies));
    }

    public void SendNotification(string message, string recipient, NotificationType type)
    {
        var strategy = _strategies.FirstOrDefault(s => s.CanHandle(type));
        
        if (strategy == null)
        {
            throw new NotSupportedException($"Notification type {type} is not supported.");
        }

        strategy.SendNotification(recipient, message);
    }
}

public class ScheduledSmsSender2 : ISmsSender, IScheduledNotificationSender
{
    public void SendSms(string phoneNumber, string message)
    {
        SendScheduledNotification(phoneNumber, message, DateTime.Now);
    }

    public void SendScheduledNotification(string recipient, string message, DateTime scheduleTime)
    {
        Console.WriteLine($"[ScheduledSmsSender] Scheduling SMS to {recipient} for {scheduleTime}: {message}");
    }
}
