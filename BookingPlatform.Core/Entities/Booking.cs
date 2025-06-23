namespace BookingPlatform.Core.Entities;

public class Booking : BaseEntity
{
    public User User { get; set; }
    public Guid UserId { get; set; }
    public List<Room> Rooms { get; set; }
    public string Remarks { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public DateOnly BookingDate { get; set; }
    public decimal TotalPriceBeforeDiscount { get; set; }
    public decimal TotalPriceAfterDiscount { get; set; }
}

