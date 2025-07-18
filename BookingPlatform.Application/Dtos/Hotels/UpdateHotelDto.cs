
namespace BookingPlatform.Application.Dtos.Hotels;

public class UpdateHotelDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Location { get; set; }
    public Guid CityId { get; set; }
    public int StarRating { get; set; }
    public string FullDescription { get; set; }
    public string BriefDescription { get; set; }
    public int PhoneNumber { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public Guid OwnerId { get; set; }
}

