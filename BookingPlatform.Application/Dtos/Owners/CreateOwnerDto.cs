namespace BookingPlatform.Application.Dtos.Owners;
public class CreateOwnerDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}

