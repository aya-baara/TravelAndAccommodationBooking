namespace BookingPlatform.Application.Dtos.Users;

public class SignUpDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Guid RoleId { get; set; }
}

