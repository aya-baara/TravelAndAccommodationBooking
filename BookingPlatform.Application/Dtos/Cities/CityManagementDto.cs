namespace BookingPlatform.Application.Dtos.Cities;

public class CityManagementDto
{
    public string Name { get; init; }
    public string Country { get; init; }
    public string PostOffice { get; init; }
    public int HotelCount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ModifiedAt { get; init; }
}

