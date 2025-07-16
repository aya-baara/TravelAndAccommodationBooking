using BookingPlatform.Application.Dtos.Cities;
using Sieve.Models;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface ICityQueryService
{
    Task<CityResponseDto> GetCityByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<CityResponseDto>> GetTopVisitedCities(int num, CancellationToken cancellationToken);
    Task<PaginatedResult<CityManagementDto>> SearchCitiesAsync(SieveModel request, CancellationToken ct);

}

