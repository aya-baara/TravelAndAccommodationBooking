using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Images.Dtos;

public class ImageResponseDto
{
    public ImageType Type { get; set; }
    public string Path { get; set; }

}

