using AutoMapper;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Owners.Commands.Update;
public class UpdateOwnerHandler : IRequestHandler<UpdateOwnerCommand>
{

    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;
    public UpdateOwnerHandler(IMapper mapper, IOwnerRepository ownerRepository)
    {
        _mapper = mapper;
        _ownerRepository = ownerRepository;
    }

    public async Task Handle(UpdateOwnerCommand request, CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetOwnerByIdAsync(request.Id)
            ?? throw new NotFoundException($"The Requested Owner Not found");
        _mapper.Map(request, owner);
        await _ownerRepository.UpdateOwner(owner);
    }
}

