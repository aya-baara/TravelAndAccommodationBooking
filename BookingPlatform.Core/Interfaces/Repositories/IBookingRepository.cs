using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IBookingRepository
{
    Task CreateBookingAsync(Booking booking);
    Task<List<Booking>> GetBookingByUserIdAsync(Guid userId);
    Task<List<Booking>> GetRecentlyBookingByUserIdAsync(Guid userId);
    
}

