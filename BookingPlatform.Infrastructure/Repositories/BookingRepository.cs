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

    public async Task<Booking> CreateBookingAsync(Booking booking)
    {
        var result = await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return result.Entity;

    }

    public Task<List<Booking>> GetBookingByUserIdAsync(Guid userId)
    {
        return _context.Bookings.Where(b => b.UserId == userId).ToListAsync();
    }

    public Task<List<Booking>> GetRecentlyBookingByUserIdAsync(Guid userId)
    {
        return _context.Bookings.Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate).Take(3).ToListAsync();
    }
}

