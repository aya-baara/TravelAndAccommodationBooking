using MediatR;

namespace BookingPlatform.Application.Hotels.Commands.Update;

public class UpdateHotelCommand :IRequest
{
    public Guid HotelId { get; set; }
    public string Name { get; init; }
    public string Location { get; init; }
    public Guid CityId { get; init; }
    public int StarRating { get; set; }
    public string FullDescription { get; init; }
    public string BriefDescription { get; init; }
    public int PhoneNumber { get; init; }
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public Guid OwnerId { get; init; }
}

