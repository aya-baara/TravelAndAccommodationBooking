using BookingPlatform.Core.Enums;

namespace BookingPlatform.Core.Entities;

public class Room : BaseEntity, IAuditableEntity
{
    public RoomType RoomType { get; set; }
    public bool IsAvailble { get; set; }
    public string Description { get; set; }
    public int PricePerNight { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public Hotel Hotel { get; set; }
    public Guid HotelId { get; set; }
    public List<Booking> Bookings { get; set; } = new();
    public List<Discount> Discounts { get; set; } = new List<Discount>();
    public List<Image> Images { get; set; } = new List<Image>();
    public DateTime CreatedAt { get; set; }
    public DateTime? ModifiedAt { get; set; }
}

