namespace BookingPlatform.Application.Dtos.Discounts;

public class DiscountResponseDto
{
    public Guid Id { get; set; }
    public Guid RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Percentage { get; set; }
}
