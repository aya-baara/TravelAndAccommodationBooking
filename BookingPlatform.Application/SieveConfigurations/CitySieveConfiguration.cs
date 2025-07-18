using BookingPlatform.Application.Dtos.Cities;
using Sieve.Services;

namespace BookingPlatform.Application.SieveConfigurations;

public class CitySieveConfiguration : ISieveCustomConfiguration
{
    public void Apply(SievePropertyMapper mapper)
    {
        mapper.Property<CityManagementDto>(x => x.Name).CanFilter().CanSort();
        mapper.Property<CityManagementDto>(x => x.Country).CanFilter().CanSort();
        mapper.Property<CityManagementDto>(x => x.PostOffice).CanFilter().CanSort();
        mapper.Property<CityManagementDto>(x => x.HotelCount).CanFilter().CanSort();
        mapper.Property<CityManagementDto>(x => x.CreatedAt).CanSort();
        mapper.Property<CityManagementDto>(x => x.ModifiedAt).CanSort();
    }
}
