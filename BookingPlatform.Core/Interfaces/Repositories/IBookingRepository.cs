using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetBookingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetRecentlyBookingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    
}

