using BookingPlatform.Core.Models;

public interface IEmailService
{
    Task SendEmailAsync(EmailMessage email);
}
