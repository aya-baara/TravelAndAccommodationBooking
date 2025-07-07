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

    public async Task<List<FeaturedHotelProjection>> GetFeaturedDealsAsync(int count, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;

        return await _context.Rooms
            .Where(r => r.Discounts.Any(d => d.StartDate <= today && d.EndDate >= today))
            .Include(r => r.Hotel)
                .ThenInclude(h => h.Thumbnail)
            .Include(r => r.Hotel.City)
            .Select(r => new
            {
                Hotel = r.Hotel,
                OriginalPrice = r.PricePerNight,
                DiscountedPrice = r.Discounts
                    .Where(d => d.StartDate <= today && d.EndDate >= today)
                    .OrderByDescending(d => d.Percentage)
                    .Select(d => r.PricePerNight * (1 - d.Percentage / 100m))
                    .FirstOrDefault()
            })
            .GroupBy(x => x.Hotel.Id)
            .Select(g => new FeaturedHotelProjection
            {
                HotelId = g.Key,
                HotelName = g.First().Hotel.Name,
                Location = g.First().Hotel.Location,
                Thumbnail = g.First().Hotel.Thumbnail ?? null,
                StarRating = g.First().Hotel.StarRating,
                OriginalPrice = g.Min(x => x.OriginalPrice),
                DiscountedPrice = g.Min(x => x.DiscountedPrice)
            })
            .OrderBy(h => h.DiscountedPrice)
            .Take(count)
            .ToListAsync(cancellationToken);
    }
    public IQueryable<Hotel> GetAllAsQueryable()
    {
        return _context.Hotels
            .Include(h => h.Thumbnail)         
            .Include(h => h.Rooms)
            .Include(h=>h.Owner);   
    }

}

