namespace BookingPlatform.Core.Entities;

public class Owner : Person
{
    public List<Hotel> Hotels { get; set; } = new List<Hotel>();
}

