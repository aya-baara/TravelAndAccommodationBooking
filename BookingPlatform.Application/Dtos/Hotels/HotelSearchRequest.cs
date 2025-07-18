namespace BookingPlatform.Application.Dtos.Hotels;

using Sieve.Models;

public class HotelSearchRequest : SieveModel
{
    public DateOnly? CheckIn { get; set; }
    public DateOnly? CheckOut { get; set; }
    public int Adults { get; set; } = 2;
    public int Children { get; set; } = 0;
    public int Rooms { get; set; } = 1;
}

