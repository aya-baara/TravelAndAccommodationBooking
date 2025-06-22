namespace BookingPlatform.Core.Entities;

public class User : Person
{
    public string Password { get; set; } = string.Empty;
}

