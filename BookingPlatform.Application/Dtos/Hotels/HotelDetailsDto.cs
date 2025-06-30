using BookingPlatform.Application.Dtos.Images;
using BookingPlatform.Application.Dtos.Reviews;

namespace BookingPlatform.Application.Dtos.Hotels;

public class HotelDetailsDto
{
    public string Name { get; set; }
    public string Location { get; set; }
    public int StarRating { get; set; }
    public double ReviewRating { get; set; }
    public string FullDescription { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public List<ReviewResponseDto> Reviews { get; set; } = new List<ReviewResponseDto>();
    public List<ImageResponseDto> Images { get; set; } = new List<ImageResponseDto>();
}

