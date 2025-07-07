using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Images;

public class UpdateImageDto
{
    public Guid Id { get; set; }
    public ImageType Type { get; set; }
    public string Path { get; set; }
    public Guid HotelId { get; set; }
    public Guid RoomId { get; set; }
}

