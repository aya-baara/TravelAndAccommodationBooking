using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Rooms;

public class RoomResponseDto
{
    public Guid Id { get; set; }
    public RoomType RoomType { get; set; }
    public string Description { get; set; }
    public int PricePerNight { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public Guid HotelId { get; set; }
}

