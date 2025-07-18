using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IBookingRepository
{
    Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetBookingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<List<Booking>> GetRecentlyBookingByUserIdAsync(Guid userId, int num, CancellationToken cancellationToken = default);
    Task<Booking?> GetBookingById(Guid bookingId, CancellationToken cancellationToken = default);
    Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut, CancellationToken ct);
    Task UpdateBookingAsync(Booking booking, CancellationToken cancellationToken = default);
    Task DeleteBookingAsync(Guid id, CancellationToken cancellationToken = default);
}

