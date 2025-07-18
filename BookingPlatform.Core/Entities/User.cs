namespace BookingPlatform.Core.Entities;

public class User : Person
{
    public Role Role { get; set; } = new Role();
    public  Guid RoleId { get; set; }
    public string Password { get; set; } = string.Empty;
}

