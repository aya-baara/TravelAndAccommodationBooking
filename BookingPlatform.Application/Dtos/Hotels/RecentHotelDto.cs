using BookingPlatform.Core.Entities;

namespace BookingPlatform.Application.Dtos.Hotels;

public class RecentHotelDto
{
    public string HotelName { get; set; }
    public string CityName { get; set; }
    public int StarRating { get; set; }
    public Image Thumbnail { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
    public decimal TotalPrice { get; set; }
}

