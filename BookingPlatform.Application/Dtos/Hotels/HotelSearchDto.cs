using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Dtos.Hotels;

public class HotelSearchDto
{
    public string Name { get; set; }
    public int StarRating { get; set; }
    public double PricePerNight { get; set; }
    public string Location { get; set; }
    public string BriefDescription { get; set; }
    public Image Thumbnail { get; set; }
}

