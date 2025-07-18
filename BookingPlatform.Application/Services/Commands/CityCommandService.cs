using AutoMapper;
using BookingPlatform.Application.Dtos.Cities;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class CityCommandService : ICityCommandService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CityCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public CityCommandService(ICityRepository cityRepository
        , IMapper mapper
        , ILogger<CityCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<CityResponseDto> CreateCityAsync(CreateCityDto dto, CancellationToken cancellationToken)
    {
        var city = _mapper.Map<City>(dto);
        var createdCity = await _cityRepository.CreateCityAsync(city, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"City created successfully with ID {createdCity.Id}");

        return _mapper.Map<CityResponseDto>(createdCity);
    }

    public async Task DeleteCityAsync(Guid id, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetCityByIdAsync(id, cancellationToken);

        if (city is null)
        {
            _logger.LogWarning($"Attempted to delete non-existent city with ID {id}");

            throw new NotFoundException("The Requested City Not found");
        }
        await _cityRepository.DeleteCityAsync(id, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"City deleted successfully with ID {id}");
    }

    public async Task UpdateCityAsync(UpdateCityDto dto, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetCityByIdAsync(dto.Id, cancellationToken);

        if (city is null)
        {
            _logger.LogWarning($"Attempted to delete non-existent city with ID {dto.Id}");

            throw new NotFoundException("The Requested City Not found");
        }
        _mapper.Map(dto, city);
        await _cityRepository.UpdateCityAsync(city);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"City updated successfully with ID {dto.Id}");
    }
}

