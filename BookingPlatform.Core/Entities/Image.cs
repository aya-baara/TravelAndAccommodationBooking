using BookingPlatform.Core.Enums;

namespace BookingPlatform.Core.Entities;

public class Image : BaseEntity
{
    public ImageType Type { get; set; }
    public string Path { get; set; }
    public Guid? HotelId { get; set; }
    public Hotel? Hotel { get; set; }
    public Guid? RoomId { get; set; }
    public Room? Room { get; set; }
}

