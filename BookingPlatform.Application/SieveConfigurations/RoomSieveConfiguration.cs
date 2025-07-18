using BookingPlatform.Application.Dtos.Rooms;
using Sieve.Services;

namespace BookingPlatform.Application.SieveConfigurations;

public class RoomSieveConfiguration : ISieveCustomConfiguration
{
    public void Apply(SievePropertyMapper mapper)
    {
        mapper.Property<RoomManagementDto>(r => r.IsAvailable).CanFilter();
        mapper.Property<RoomManagementDto>(r => r.AdultCapacity).CanFilter().CanSort();
        mapper.Property<RoomManagementDto>(r => r.ChildrenCapacity).CanFilter().CanSort();
        mapper.Property<RoomManagementDto>(r => r.CreatedAt).CanSort();
        mapper.Property<RoomManagementDto>(r => r.ModifiedAt).CanSort();
    }
}


