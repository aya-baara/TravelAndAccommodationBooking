using MediatR;

namespace BookingPlatform.Application.Owners.Commands.Delete;

public class DeleteOwnerCommand : IRequest
{
    public Guid Id { get; set; }
}

