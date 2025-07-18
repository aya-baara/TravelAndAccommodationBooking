using AutoMapper;
using BookingPlatform.Application.Dtos.Owners;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces;
using Microsoft.Extensions.Logging;

public class OwnerCommandService : IOwnerCommandService
{
    private readonly IOwnerRepository _ownerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<OwnerCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public OwnerCommandService(IOwnerRepository ownerRepository
        , IMapper mapper
        , ILogger<OwnerCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _ownerRepository = ownerRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<OwnerResponseDto> CreateOwnerAsync(CreateOwnerDto dto, CancellationToken cancellationToken)
    {
        var ownerEntity = _mapper.Map<Owner>(dto);
        var createdOwner = await _ownerRepository.CreateOwnerAsync(ownerEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Owner created successfully with ID {OwnerId}", createdOwner.Id);

        return _mapper.Map<OwnerResponseDto>(createdOwner);
    }

    public async Task DeleteOwnerAsync(Guid id, CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetOwnerByIdAsync(id);
        if (owner is null)
        {
            _logger.LogWarning("Attempted to delete non-existent owner with ID {OwnerId}", id);
            throw new NotFoundException("The Requested Owner Not found");
        }

        await _ownerRepository.DeleteOwnerById(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Owner deleted successfully with ID {OwnerId}", id);
    }

    public async Task UpdateOwnerAsync(UpdateOwnerDto dto, CancellationToken cancellationToken)
    {
        var owner = await _ownerRepository.GetOwnerByIdAsync(dto.Id);
        if (owner is null)
        {
            _logger.LogWarning("Attempted to update non-existent owner with ID {OwnerId}", dto.Id);
            throw new NotFoundException("The Requested Owner Not found");
        }

        _mapper.Map(dto, owner);
        await _ownerRepository.UpdateOwner(owner);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Owner updated successfully with ID {OwnerId}", dto.Id);
    }
}
