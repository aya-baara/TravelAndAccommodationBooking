using BookingPlatform.Application.Dtos.Hotels;
using Sieve.Models;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IHotelQueryService
{
    Task<HotelDetailsDto> GetHotelDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<FeaturedHotelDto>> GetFeaturedDealsAsync(int count, CancellationToken cancellationToken);
    Task<PaginatedResult<HotelSearchDto>> SearchHotelsAsync(HotelSearchRequest request, CancellationToken ct);
    Task<PaginatedResult<HotelManagementDto>> SearchHotelsAdminAsync(SieveModel request, CancellationToken ct);
}

