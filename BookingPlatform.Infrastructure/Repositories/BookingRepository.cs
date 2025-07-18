using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;
    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Booking> CreateBookingAsync(Booking booking, CancellationToken cancellationToken = default)
    {
        var result = await _context.Bookings.AddAsync(booking, cancellationToken);
        return result.Entity;

    }

    public Task<List<Booking>> GetBookingByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return _context.Bookings.Where(b => b.UserId == userId).ToListAsync(cancellationToken);
    }

    public async Task<List<Booking>> GetRecentlyBookingByUserIdAsync(Guid userId, int num
        , CancellationToken cancellationToken = default)
    {
        return await _context.Bookings
         .Include(b => b.Rooms).ThenInclude(r => r.Hotel).ThenInclude(h => h.Thumbnail)
         .Include(b => b.Rooms).ThenInclude(r => r.Hotel.City)
         .Include(b => b.Invoice)
         .Where(b => b.UserId == userId)
         .OrderByDescending(b => b.BookingDate)
         .Take(num)
         .ToListAsync(cancellationToken);
    }
    public async Task<Booking?> GetBookingById(Guid bookingId, CancellationToken cancellationToken)
    {
        return await _context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

    }
    public async Task<bool> IsRoomAvailableAsync(Guid roomId, DateTime checkIn, DateTime checkOut, CancellationToken ct)
    {
        return !await _context.Bookings
            .Where(b => b.Rooms.Any(r => r.Id == roomId))
            .AnyAsync(b =>
                (checkIn < b.CheckOut && checkOut > b.CheckIn), ct);
    }
    public async Task UpdateBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        _context.Bookings.Update(booking);
    }
    public async Task DeleteBookingAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var booking = await GetBookingById(id, cancellationToken);
        if (booking != null)
        {
            _context.Bookings.Remove(booking);
        }

    }
}

