namespace BookingPlatform.Application.Dtos.Discounts;

public class CreateDiscountDto
{
    public Guid RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Percentage { get; set; }
}
