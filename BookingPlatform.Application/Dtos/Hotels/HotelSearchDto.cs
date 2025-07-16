using BookingPlatform.Application.Dtos.Rooms;
using BookingPlatform.Core.Entities;
using Sieve.Attributes;

namespace BookingPlatform.Application.Dtos.Hotels;

public class HotelSearchDto
{
    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int StarRating { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public double PricePerNight { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Location { get; set; }

    [Sieve(CanFilter = true)]
    public string BriefDescription { get; set; }
    public Image Thumbnail { get; set; }
    public List<RoomResponseDto> Rooms { get; set; } 
}

