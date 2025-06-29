using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Enums;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Models;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class HotelRepository : IHotelRepository
{
    private readonly AppDbContext _context;

    public HotelRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Hotel> CreateHotelAsync(Hotel hotel)
    {
        var result = await _context.Hotels.AddAsync(hotel);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task DeleteHotelByIdAsync(Guid hotelId)
    {
        var hotel = await GetHotelByIdAsync(hotelId);
        if (hotel != null)
        {
            _context.Hotels.Remove(hotel);
        }
        await _context.SaveChangesAsync();
    }

    public async Task<Hotel?> GetHotelByIdAsync(Guid hotelId)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId);
        if (hotel != null)
        {
            var hotelThumbnail = await _context.Images
                .FirstOrDefaultAsync(i => i.HotelId == hotelId && i.Type == ImageType.Thumbnail);
            hotel.Thumbnail = hotelThumbnail;
        }
        return hotel;
    }

    public async Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(Guid cityId, int page, int size)
    {
        var totalCount = await _context.Hotels.Where(h => h.CityId == cityId).CountAsync();

        var items = await _context.Hotels
            .Where(h => h.CityId == cityId)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();

        foreach (var item in items)
        {
            var hotelThumbnail = _context.Images
           .FirstOrDefault(i => i.HotelId == item.Id && i.Type == ImageType.Thumbnail);
            item.Thumbnail = hotelThumbnail;
        }

        return new PaginatedResult<Hotel>(items, totalCount, page, size);
    }

    public async Task UpdateHotelAsync(Hotel hotel)
    {
        _context.Hotels.Update(hotel);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateHotelRateAsync(Guid hotelId, double newRate)
    {
        var hotel = await GetHotelByIdAsync(hotelId);
        if (hotel != null)
        {
            hotel.ReviewRating = newRate;
            await _context.SaveChangesAsync();
        }
    }
    public async Task<HotelRatingStats?> GetRatingStatsByHotelIdAsync(Guid hotelId)
    {
        var stats = await _context.Reviews
            .Where(r => r.HotelId == hotelId)
            .GroupBy(_ => 1)
            .Select(g => new HotelRatingStats
            {
                Count = g.Count(),
                Sum = g.Sum(r => r.Rate)
            })
            .FirstOrDefaultAsync();

        return stats;
    }

}

