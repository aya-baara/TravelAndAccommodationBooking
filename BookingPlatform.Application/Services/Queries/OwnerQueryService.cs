using AutoMapper;
using BookingPlatform.Application.Dtos.Owners;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class OwnerQueryService : IOwnerQueryService
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<OwnerQueryService> _logger;

    public OwnerQueryService(IOwnerRepository ownerRepository, IMapper mapper, ILogger<OwnerQueryService> logger)
    {
        _ownerRepository = ownerRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<OwnerResponseDto> GetOwnerByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetOwnerByIdAsync(id, cancellationToken);
        if (owner is null)
        {
            _logger.LogWarning($"Owner with ID {id} not found");
            throw new NotFoundException("The Requested Owner Not found");
        }

        _logger.LogInformation($"Successfully retrieved owner with ID {id}");

        return _mapper.Map<OwnerResponseDto>(owner);
    }
}


