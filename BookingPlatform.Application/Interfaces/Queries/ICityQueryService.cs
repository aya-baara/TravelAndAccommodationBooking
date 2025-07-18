using BookingPlatform.Application.Dtos.Cities;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface ICityQueryService
{
    Task<CityResponseDto> GetCityByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<CityResponseDto>> GetTopVisitedCities(int num, CancellationToken cancellationToken);
    Task<PaginatedResult<CityManagementDto>> SearchCitiesAsync(CityAdminSearchRequest request, CancellationToken ct);

}

