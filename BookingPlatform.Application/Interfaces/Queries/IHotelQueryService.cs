using BookingPlatform.Application.Dtos.Hotels;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IHotelQueryService
{
    Task<HotelDetailsDto> GetHotelDetailsByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<FeaturedHotelDto>> GetFeaturedDealsAsync(int count, CancellationToken cancellationToken);
    Task<PaginatedResult<HotelSearchDto>> SearchHotelsAsync(HotelSearchRequest request, CancellationToken ct);
    Task<PaginatedResult<HotelManagementDto>> SearchHotelsAdminAsync(HotelAdminSearchRequest request, CancellationToken ct);
}

