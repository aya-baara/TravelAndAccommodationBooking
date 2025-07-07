using BookingPlatform.Application.Dtos.Bookings;
using BookingPlatform.Application.Dtos.Hotels;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IBookingQueryService
{
    Task<BookingResponseDto> GetBookingByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<RecentHotelDto>> GetRecentHotelsForUserAsync(Guid userId, int count, CancellationToken cancellationToken);
}

