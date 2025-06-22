namespace BookingPlatform.Core.Entities;

public class User : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int PhoneNumber { get; set; }
    public string Password { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public Role Role { get; set; } = new Role();

}

