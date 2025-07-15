using AutoMapper;
using BookingPlatform.Application.Dtos.Hotels;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class HotelCommandService : IHotelCommandService
{
    private readonly IHotelRepository _hotelRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IOwnerRepository _ownerRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<HotelCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public HotelCommandService(IHotelRepository hotelRepository
        , ICityRepository cityRepository
        , IOwnerRepository ownerRepository
        , IMapper mapper
        , ILogger<HotelCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _hotelRepository = hotelRepository;
        _cityRepository = cityRepository;
        _ownerRepository = ownerRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<HotelResponseDto> CreateHotelAsync(CreateHotelDto dto, CancellationToken cancellationToken)
    {
        var hotel = _mapper.Map<Hotel>(dto);

        var city = await _cityRepository.GetCityByIdAsync(dto.CityId);
        if (city is null)
        {
            _logger.LogWarning("Attempted to create a hotel with non-existent CityId {CityId}", dto.CityId);
            throw new NotFoundException($"City with id {dto.CityId} not found");
        }

        var owner = await _ownerRepository.GetOwnerByIdAsync(dto.OwnerId);
        if (owner is null)
        {
            _logger.LogWarning("Attempted to create a hotel with non-existent OwnerId {OwnerId}", dto.OwnerId);
            throw new NotFoundException($"Owner with id {dto.OwnerId} not found");
        }

        var created = await _hotelRepository.CreateHotelAsync(hotel,cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation("Hotel '{HotelName}' created successfully with ID {HotelId}", created.Name, created.Id);

        return _mapper.Map<HotelResponseDto>(created);
    }


    public async Task DeleteHotelAsync(Guid id, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(id,cancellationToken);
        if (hotel is null)
        {
            _logger.LogWarning($"Attempted to delete non-existent hotel with ID {id}");
            throw new NotFoundException("The Requested Hotel Not found");
        }
        await _hotelRepository.DeleteHotelByIdAsync(id);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation($"Hotel deleted successfully with ID {id}");
    }

    public async Task UpdateHotelAsync(UpdateHotelDto dto, CancellationToken cancellationToken)
    {
        var hotel = await _hotelRepository.GetHotelByIdAsync(dto.Id);
        if (hotel is null)
        {
            _logger.LogWarning($"Attempted to update non-existent hotel with ID {dto.Id}");
            throw new NotFoundException("The Requested Hotel Not found");
        }
        _mapper.Map(dto, hotel);
        await _hotelRepository.UpdateHotelAsync(hotel);
        await _unitOfWork.SaveChangesAsync();
        _logger.LogInformation($"Hotel Updated successfully with ID {dto.Id}");
    }
}

