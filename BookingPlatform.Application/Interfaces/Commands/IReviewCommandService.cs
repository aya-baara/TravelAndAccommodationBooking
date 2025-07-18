using BookingPlatform.Application.Dtos.Reviews;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IReviewCommandService
{
    Task<ReviewResponseDto> CreateReviewAsync(CreateReviewDto dto, CancellationToken cancellationToken);
    Task UpdateReview(UpdateReviewDto dto, CancellationToken cancellationToken);
    Task DeleteReview(Guid id, Guid userId, CancellationToken cancellationToken);
}

