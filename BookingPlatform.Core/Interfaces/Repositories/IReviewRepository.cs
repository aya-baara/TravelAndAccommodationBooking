using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IReviewRepository
{
    Task CreateReviewAsync(Review review);
    Task<Review?> GetReviewByIdAsync(Guid reviewId);
    Task<PaginatedResult<Review>> GetReviewsByHotelIdAsync(Guid hotelId, int page, int size);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewById(Guid reviewId);
}
