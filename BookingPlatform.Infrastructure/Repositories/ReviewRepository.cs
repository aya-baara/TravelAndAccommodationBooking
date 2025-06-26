using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly AppDbContext _context;

    public ReviewRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task CreateReviewAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteReviewById(Guid reviewId)
    {
        var review = await GetReviewByIdAsync(reviewId);
        if (review != null)
        {
            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Review?> GetReviewByIdAsync(Guid reviewId)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId);
    }

    public async Task<PaginatedResult<Review>> GetReviewsByHotelIdAsync(Guid hotelId, int page, int size)
    {
        var totalCount = await _context.Reviews.Where(r=>r.HotelId==hotelId).CountAsync();

        var items = await _context.Reviews
            .Where(r => r.HotelId == hotelId)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();
        return new PaginatedResult<Review>(items, totalCount, page, size);
    }

    public async Task UpdateReviewAsync(Review review)
    {
        _context.Reviews.Update(review);
        await _context.SaveChangesAsync();
    }
}

