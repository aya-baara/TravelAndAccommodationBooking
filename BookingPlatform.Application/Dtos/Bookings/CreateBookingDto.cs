using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Dtos.Bookings;

public class CreateBookingDto
{
    public List<Guid> RoomIds { get; set; }
    public string? Remarks { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}

