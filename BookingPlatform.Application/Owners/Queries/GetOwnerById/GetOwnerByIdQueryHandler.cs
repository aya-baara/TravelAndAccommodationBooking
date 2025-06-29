using AutoMapper;
using BookingPlatform.Application.Owners.Dtos;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using MediatR;

namespace BookingPlatform.Application.Owners.Queries.GetOwnerById;

public class GetOwnerByIdQueryHandler : IRequestHandler<GetOwnerByIdQuery, OwnerResponseDto>
{
    private readonly IMapper _mapper;
    private readonly IOwnerRepository _ownerRepository;

    public GetOwnerByIdQueryHandler(IMapper mapper, IOwnerRepository ownerRepository)
    {
        _mapper = mapper;
        _ownerRepository = ownerRepository;
    }

    public async Task<OwnerResponseDto> Handle(GetOwnerByIdQuery request, CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetOwnerByIdAsync(request.Id) ??
            throw new NotFoundException($"The Requested Owner Not found");
        return _mapper.Map<OwnerResponseDto>(owner);
    }
}


