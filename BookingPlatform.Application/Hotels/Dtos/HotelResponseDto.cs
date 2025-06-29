using BookingPlatform.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace BookingPlatform.Application.Hotels.Dtos;

public class HotelResponseDto
{
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
    public DateTime CreateAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

