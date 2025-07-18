using BookingPlatform.Core.Entities;
using Sieve.Services;

namespace BookingPlatform.Application.SieveConfigurations;

public class HotelSieveConfiguration : ISieveCustomConfiguration
{
    public void Apply(SievePropertyMapper mapper)
    {
        mapper.Property<Hotel>(h => h.StarRating).CanFilter().CanSort();
        mapper.Property<Hotel>(h => h.Location).CanFilter().CanSort();
        mapper.Property<Hotel>(h => h.Name).CanFilter().CanSort();
        mapper.Property<Hotel>(h => h.BriefDescription).CanFilter();
        mapper.Property<Hotel>(h => h.Rooms.First().PricePerNight).CanSort();
    }
}

