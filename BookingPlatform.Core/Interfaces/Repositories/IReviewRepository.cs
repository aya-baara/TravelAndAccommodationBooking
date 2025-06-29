using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<Review> CreateReviewAsync(Review review);
    Task<Review?> GetReviewByIdAsync(Guid reviewId);
    Task<PaginatedResult<Review>> GetReviewsByHotelIdAsync(Guid hotelId, int page = 1, int size = 20);
    Task UpdateReviewAsync(Review review);
    Task DeleteReviewById(Guid reviewId);
}
