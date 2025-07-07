using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Services;

namespace BookingPlatform.Application.Services.Helpers;

public class BookingHtmlBuilder : IBookingHtmlBuilder
{
    public string BuildConfirmationHtml(Booking booking)
    {
        return $@"
            <h1>Booking Confirmation</h1>
            <p><strong>Confirmation #:</strong> {booking.Id}</p>
            <p><strong>Hotel:</strong> {booking.Rooms.First().Hotel.Name}</p>
            <p><strong>Address:</strong> {booking.Rooms.First().Hotel.Location}</p>
            <p><strong>Check-in:</strong> {booking.CheckIn}</p>
            <p><strong>Check-out:</strong> {booking.CheckOut}</p>
            <p><strong>Total:</strong> {booking.TotalPriceAfterDiscount}</p>";
    }
}
