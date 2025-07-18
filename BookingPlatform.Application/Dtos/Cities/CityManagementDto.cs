using Sieve.Attributes;

namespace BookingPlatform.Application.Dtos.Cities;

public class CityManagementDto
{
    [Sieve(CanFilter = true, CanSort = true)]
    public string Name { get; init; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string Country { get; init; }

    [Sieve(CanFilter = true, CanSort = true)]
    public string PostOffice { get; init; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int HotelCount { get; init; }

    [Sieve(CanSort = true)]
    public DateTime CreatedAt { get; init; }

    [Sieve(CanSort = true)]
    public DateTime? ModifiedAt { get; init; }
}

