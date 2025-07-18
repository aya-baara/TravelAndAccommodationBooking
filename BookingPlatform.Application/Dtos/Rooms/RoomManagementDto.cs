namespace BookingPlatform.Application.Dtos.Rooms;

public class RoomManagementDto
{
    public Guid Id { get; set; }
    public bool IsAvailable { get; set; }
    public int AdultCapacity { get; set; }
    public int ChildrenCapacity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

