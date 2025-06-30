using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Images;

public class ImageResponseDto
{
    public ImageType Type { get; set; }
    public string Path { get; set; }

}

