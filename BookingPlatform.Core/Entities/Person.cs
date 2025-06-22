namespace BookingPlatform.Core.Entities;

public class Person : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public Role Role { get; set; } = new Role();
    public DateTime DateOfBirth { get; set; }
}

