using BookingPlatform.Application.Owners.Dtos;
using MediatR;

namespace BookingPlatform.Application.Owners.Commands;

public class CreateOwnerCommand : IRequest<OwnerResponseDto>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int PhoneNumber { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
}
