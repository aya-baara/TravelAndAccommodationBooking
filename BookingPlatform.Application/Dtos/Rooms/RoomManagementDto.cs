using Sieve.Attributes;

namespace BookingPlatform.Application.Dtos.Rooms;

public class RoomManagementDto
{
    public Guid Id { get; set; }

    [Sieve(CanFilter = true)]
    public bool IsAvailable { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int AdultCapacity { get; set; }

    [Sieve(CanFilter = true, CanSort = true)]
    public int ChildrenCapacity { get; set; }

    [Sieve(CanSort = true)]
    public DateTime CreatedAt { get; set; }

    [Sieve(CanSort = true)]
    public DateTime ModifiedAt { get; set; }
}

