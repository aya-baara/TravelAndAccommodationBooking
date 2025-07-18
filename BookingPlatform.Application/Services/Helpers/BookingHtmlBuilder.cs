using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;

namespace BookingPlatform.Application.Services.Helpers;

public class BookingHtmlBuilder : IBookingHtmlBuilder
{
    private readonly IHotelRepository hotelRepository;

    public BookingHtmlBuilder(IHotelRepository hotelRepository)
    {
        this.hotelRepository = hotelRepository;
    }

    public async Task<string> BuildConfirmationHtml(Booking booking)
    {
        var hotel = await hotelRepository.GetHotelByIdAsync(booking.Rooms.First().HotelId);
        return $@"
            <h1>Booking Confirmation</h1>
            <p><strong>Confirmation #:</strong> {booking.Id}</p>
            <p><strong>Hotel:</strong> {hotel.Name} </p>
            <p><strong>Address:</strong> {hotel.Location}</p>
            <p><strong>Check-in:</strong> {booking.CheckIn}</p>
            <p><strong>Check-out:</strong> {booking.CheckOut}</p>
            <p><strong>Total:</strong> {booking.TotalPriceAfterDiscount}</p>";
    }
}
