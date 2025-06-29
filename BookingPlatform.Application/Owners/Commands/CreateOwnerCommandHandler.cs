using AutoMapper;
using BookingPlatform.Application.Owners.Commands;
using BookingPlatform.Application.Owners.Dtos;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Owners.Handlers.CommandHandlers;

public class CreateOwnerCommandHandler : IRequestHandler<CreateOwnerCommand, OwnerResponseDto>
{
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;

    public CreateOwnerCommandHandler(IMapper mapper, IOwnerRepository ownerRepository)
    {
        _mapper = mapper;
        _ownerRepository = ownerRepository;
    }

    public async Task<OwnerResponseDto> Handle(CreateOwnerCommand request, CancellationToken cancellationToken)
    {
        var ownerEntity = _mapper.Map<Owner>(request);
        var createdOwner = await _ownerRepository.CreateOwnerAsync(ownerEntity);
        return _mapper.Map<OwnerResponseDto>(createdOwner);
    }
}

