using AutoMapper;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Owners.Commands.Delete;

public class DeleteOwnerCommandHandler : IRequestHandler<DeleteOwnerCommand>
{
    private readonly IOwnerRepository _ownerRepository;
    public DeleteOwnerCommandHandler(IOwnerRepository ownerRepository)
    {
        _ownerRepository = ownerRepository;
    }
    public async Task Handle(DeleteOwnerCommand request, CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetOwnerByIdAsync(request.Id)
            ?? throw new NotFoundException($"The Requested Owner  Not found");
        await _ownerRepository.DeleteOwnerById(request.Id);
    }
}

