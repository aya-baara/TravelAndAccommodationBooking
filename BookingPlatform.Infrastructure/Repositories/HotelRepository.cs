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

    public async Task<Hotel> CreateHotelAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        var result = await _context.Hotels.AddAsync(hotel, cancellationToken);
        return result.Entity;
    }

    public async Task DeleteHotelByIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await GetHotelByIdAsync(hotelId, cancellationToken);
        if (hotel != null)
        {
            _context.Hotels.Remove(hotel);
        }
    }

    public async Task<Hotel?> GetHotelByIdAsync(Guid hotelId, CancellationToken cancellationToken = default)
    {
        var hotel = await _context.Hotels.FirstOrDefaultAsync(h => h.Id == hotelId, cancellationToken);
        if (hotel != null)
        {
            var hotelThumbnail = await _context.Images
                .FirstOrDefaultAsync(i => i.HotelId == hotelId && i.Type == ImageType.Thumbnail, cancellationToken);
            hotel.Thumbnail = hotelThumbnail;
        }
        return hotel;
    }

    public async Task<PaginatedResult<Hotel>> GetHotelsByCityIdAsync(Guid cityId, int page, int size
        , CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Hotels.Where(h => h.CityId == cityId).CountAsync(cancellationToken);

        var items = await _context.Hotels
            .Where(h => h.CityId == cityId)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);

        foreach (var item in items)
        {
            var hotelThumbnail = _context.Images
           .FirstOrDefault(i => i.HotelId == item.Id && i.Type == ImageType.Thumbnail);
            item.Thumbnail = hotelThumbnail;
        }

        return new PaginatedResult<Hotel>(items, totalCount, page, size);
    }

    public async Task UpdateHotelAsync(Hotel hotel, CancellationToken cancellationToken = default)
    {
        _context.Hotels.Update(hotel);
    }

    public async Task UpdateHotelRateAsync(Guid hotelId, double newRate, CancellationToken cancellationToken = default)
    {
        var hotel = await GetHotelByIdAsync(hotelId, cancellationToken);
        if (hotel != null)
        {
            hotel.ReviewRating = newRate;
        }
    }
    public async Task<HotelRatingStats?> GetRatingStatsByHotelIdAsync(Guid hotelId
        , CancellationToken cancellationToken = default)
    {
        var stats = await _context.Reviews
            .Where(r => r.HotelId == hotelId)
            .GroupBy(_ => 1)
            .Select(g => new HotelRatingStats
            {
                Count = g.Count(),
                Sum = g.Sum(r => r.Rate)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats;
    }

}

