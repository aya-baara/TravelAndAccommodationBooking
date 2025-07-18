namespace BookingPlatform.Application.Dtos.Owners;

public class OwnerResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateOnly DateOfBirth { get; set; }
}

