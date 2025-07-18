namespace BookingPlatform.Application.Dtos.Cities;

public class CityResponseDto
{
    public Guid CityId { get; set; }
    public string Name { get; set; } 
    public string Country { get; set; } 
    public string PostOffice { get; set; }
    public string Description { get; set; } 
}

