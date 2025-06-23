namespace BookingPlatform.Core.Entities;

public class Discount : BaseEntity
{
    public Room Room { get; set; }
    public Guid RoomId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Percentage { get; set; }
}

