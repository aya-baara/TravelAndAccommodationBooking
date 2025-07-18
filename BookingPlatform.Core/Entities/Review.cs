namespace BookingPlatform.Core.Entities;

public class Review : BaseEntity
{
    public User User { get; set; }
    public Guid UserId { get; set; }
    public string Comment { get; set; }
    public double Rate { get; set; }
    public Hotel Hotel { get; set; }
    public Guid HotelId { get; set; }
}

