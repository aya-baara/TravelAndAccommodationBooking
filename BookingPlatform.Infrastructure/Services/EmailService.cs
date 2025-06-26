namespace BookingPlatform.Infrastructure.Services;

using BookingPlatform.Core.Models;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;

public class EmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(EmailMessage email)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(email.From ?? _settings.Username));
        message.To.Add(MailboxAddress.Parse(email.To));
        message.Subject = email.Subject;

        var builder = new BodyBuilder
        {
            HtmlBody = email.IsHtml ? email.Body : null,
            TextBody = email.IsHtml ? null : email.Body
        };

        if (email.Attachments != null)
        {
            foreach (var attachment in email.Attachments)
            {
                builder.Attachments.Add(attachment.FileName, attachment.Content,
                    ContentType.Parse(attachment.ContentType));
            }
        }

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_settings.Server, _settings.Port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_settings.Username, _settings.Password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);
    }
}