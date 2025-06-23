namespace BookingPlatform.Core.Entities;

public class City : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PostOffice { get; set; } = string.Empty;
    public String Description { get; set; } = string.Empty;
    public List<Hotel> Hotels { get; set; } = new List<Hotel>();
    public DateTime CreateAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}

