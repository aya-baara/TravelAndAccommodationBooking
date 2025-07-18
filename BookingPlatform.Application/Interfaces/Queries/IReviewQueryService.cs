using BookingPlatform.Application.Dtos.Reviews;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IReviewQueryService
{
    Task<ReviewResponseDto> GetReviewByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<PaginatedResult<ReviewResponseDto>> GetHotelReviews(Guid id, CancellationToken cancellationToken, int page, int size);
}

