using BookingPlatform.Application.Dtos.Hotels;
using Sieve.Services;

namespace BookingPlatform.Application.SieveConfigurations;

public class HotelManagementSieveConfiguration : ISieveCustomConfiguration
{
    public void Apply(SievePropertyMapper mapper)
    {
        mapper.Property<HotelManagementDto>(x => x.Name).CanFilter().CanSort();
        mapper.Property<HotelManagementDto>(x => x.StarRating).CanFilter().CanSort();
        mapper.Property<HotelManagementDto>(x => x.OwnerName).CanFilter().CanSort();
        mapper.Property<HotelManagementDto>(x => x.RoomCount).CanFilter().CanSort();
        mapper.Property<HotelManagementDto>(x => x.CreatedAt).CanSort();
        mapper.Property<HotelManagementDto>(x => x.ModifiedAt).CanSort();
    }
}

