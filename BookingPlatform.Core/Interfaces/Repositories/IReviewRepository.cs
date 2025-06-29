using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IReviewRepository
{
    Task<Review> CreateReviewAsync(Review review, CancellationToken cancellationToken = default);
    Task<Review?> GetReviewByIdAsync(Guid reviewId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Review>> GetReviewsByHotelIdAsync(Guid hotelId, int page = 1, int size = 20
        , CancellationToken cancellationToken = default);
    Task UpdateReviewAsync(Review review, CancellationToken cancellationToken = default);
    Task DeleteReviewById(Guid reviewId, CancellationToken cancellationToken = default);
>>>>>>> Infrastructure
}
