namespace Notification.Appication.Infrastructure.Services;

public interface IEmailService
{
    Task SendAsync(string recipient, string subject, string message);
}
