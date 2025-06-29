using MediatR;

namespace BookingPlatform.Application.Owners.Commands.Update;

public class UpdateOwnerCommand : IRequest
{
    public Guid Id { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int PhoneNumber { get; init; }
    public DateTime DateOfBirth { get; init; }
}

