using AutoMapper;
using BookingPlatform.Application.Dtos.Cities;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Application.Services.Commands;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Sieve.Services;
using Microsoft.EntityFrameworkCore;
using Sieve.Models;


namespace BookingPlatform.Application.Services.Queries;

public class CityQueryService : ICityQueryService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CityCommandService> _logger;
    private readonly ISieveProcessor _sieve;


    public CityQueryService(ICityRepository cityRepository
        , IMapper mapper
        , ILogger<CityCommandService> logger
        , ISieveProcessor sieveProcessor)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
        _logger = logger;
        _sieve = sieveProcessor;
    }

    public async Task<CityResponseDto> GetCityByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetCityByIdAsync(id, cancellationToken);
        if (city is null)
        {
            _logger.LogWarning($"City with ID {id} not found");
            throw new NotFoundException("The Requested City Not found");
        }

        _logger.LogInformation($"Successfully retrieved city with ID {id}");

        return _mapper.Map<CityResponseDto>(city);
    }

    public async Task<List<CityResponseDto>> GetTopVisitedCities(int num, CancellationToken cancellationToken)
    {
        var cities = await _cityRepository.GetTopBookedCitiesAsync(num, cancellationToken);

        _logger.LogInformation($"Successfully The Top {num} visited cities ");
        return _mapper.Map<List<CityResponseDto>>(cities);
    }
    public async Task<PaginatedResult<CityManagementDto>> SearchCitiesAsync(SieveModel request, CancellationToken ct)
    {
        var query = _cityRepository.GetAllAsQueryable()
            .Select(c => new CityManagementDto
            {
                Name = c.Name,
                Country = c.Country,
                PostOffice = c.PostOffice,
                HotelCount = c.Hotels.Count,
                CreatedAt = c.CreatedAt,
                ModifiedAt = c.ModifiedAt
            });

        var filtered = _sieve.Apply(request, query);

        var total = await filtered.CountAsync(ct);
        var data = await filtered.ToListAsync(ct);

        _logger.LogInformation($"Successfully retrieved Cities search result");

        return new PaginatedResult<CityManagementDto>(data, total, request.Page ?? 1, request.PageSize ?? 10);
    }

}

