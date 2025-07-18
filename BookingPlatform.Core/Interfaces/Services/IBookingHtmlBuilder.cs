using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Services;

public interface IBookingHtmlBuilder
{
    Task<string> BuildConfirmationHtml(Booking booking);
}

