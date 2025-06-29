using BookingPlatform.Application.Owners.Dtos;
using MediatR;

namespace BookingPlatform.Application.Owners.Commands.Create;

public class CreateOwnerCommand : IRequest<OwnerResponseDto>
{
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int PhoneNumber { get; init; }
    public string Email { get; init; }
    public DateTime DateOfBirth { get; init; }
}
