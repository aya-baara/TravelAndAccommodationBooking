namespace BookingPlatform.Application.Dtos.Hotels;

public class HotelManagementDto
{
    public string Name { get; set; }
    public int StarRating { get; set; }
    public string OwnerName { get; set; }
    public int RoomCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

