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

    public async Task<Review> CreateReviewAsync(Review review, CancellationToken cancellationToken = default)
    {
        var result=await _context.Reviews.AddAsync(review, cancellationToken);
        return result.Entity;

    }

    public async Task DeleteReviewById(Guid reviewId, CancellationToken cancellationToken = default)
    {
        var review = await GetReviewByIdAsync(reviewId,cancellationToken);
        if (review != null)
        {
            _context.Reviews.Remove(review);
        }
    }

    public async Task<Review?> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default)
    {
        return await _context.Reviews.FirstOrDefaultAsync(r => r.Id == reviewId,cancellationToken);
    }

    public async Task<PaginatedResult<Review>> GetReviewsByHotelIdAsync(Guid hotelId, int page, int size
        , CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Reviews.Where(r=>r.HotelId==hotelId).CountAsync(cancellationToken);

        var items = await _context.Reviews
            .Where(r => r.HotelId == hotelId)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
        return new PaginatedResult<Review>(items, totalCount, page, size);
    }

    public async Task UpdateReviewAsync(Review review, CancellationToken cancellationToken = default)
    {
        _context.Reviews.Update(review);
    }
}

