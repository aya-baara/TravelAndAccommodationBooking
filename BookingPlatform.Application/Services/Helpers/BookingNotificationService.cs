using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Services;
using BookingPlatform.Core.Models;

namespace BookingPlatform.Application.Services.Helpers;

public class BookingNotificationService : IBookingNotificationService
{
    private readonly IPdfService _pdfService;
    private readonly IEmailService _emailService;
    private readonly IBookingHtmlBuilder _htmlBuilder;

    public BookingNotificationService(IPdfService pdfService
        , IEmailService emailService
        , IBookingHtmlBuilder bookingHtmlBuilder)
    {
        _pdfService = pdfService;
        _emailService = emailService;
        _htmlBuilder = bookingHtmlBuilder;
    }

    public async Task SendBookingConfirmationAsync(Booking booking, CancellationToken ct)
    {
        var html = _htmlBuilder.BuildConfirmationHtml(booking);
        var pdf = _pdfService.GeneratePdfFromHtml(html);

        var email = new EmailMessage
        {
            To = booking.User.Email,
            Subject = "Booking Confirmation",
            Body = "Your booking is confirmed.",
            Attachments = new List<EmailAttachment>
            {
                new EmailAttachment { FileName = "confirmation.pdf", Content = pdf }
            }
        };

        await _emailService.SendEmailAsync(email);
    }
}

