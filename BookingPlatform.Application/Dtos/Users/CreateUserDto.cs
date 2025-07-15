namespace BookingPlatform.Application.Dtos.Users;

public class CreateUserDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public  Guid RoleId { get; set; }
}
