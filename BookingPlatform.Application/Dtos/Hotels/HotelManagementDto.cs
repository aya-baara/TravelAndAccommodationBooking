using Sieve.Attributes;

namespace BookingPlatform.Application.Dtos.Hotels;

public class HotelManagementDto
{
    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int StarRating { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string OwnerName { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int RoomCount { get; set; }

    [Sieve(CanSort = true)]
    public DateTime CreatedAt { get; set; }

    [Sieve(CanSort = true)]
    public DateTime ModifiedAt { get; set; }
}

