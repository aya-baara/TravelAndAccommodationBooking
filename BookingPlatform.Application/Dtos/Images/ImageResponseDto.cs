using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Images;

public class ImageResponseDto
{
    public Guid Id { get; set; }
    public ImageType Type { get; set; }
    public string Path { get; set; }

}

