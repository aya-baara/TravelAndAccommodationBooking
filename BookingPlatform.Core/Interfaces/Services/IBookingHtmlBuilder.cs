using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Services;

public interface IBookingHtmlBuilder
{
    string BuildConfirmationHtml(Booking booking);
}

