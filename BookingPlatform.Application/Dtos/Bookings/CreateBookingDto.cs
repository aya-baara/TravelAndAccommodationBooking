using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Dtos.Bookings;

public class CreateBookingDto
{
    public Guid UserId { get; set; }
    public List<Room> Rooms { get; set; }
    public string Remarks { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public DateOnly BookingDate { get; set; }
    public decimal TotalPriceBeforeDiscount { get; set; }
    public decimal TotalPriceAfterDiscount { get; set; }
}

