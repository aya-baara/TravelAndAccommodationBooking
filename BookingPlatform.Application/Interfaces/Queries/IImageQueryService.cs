using BookingPlatform.Application.Dtos.Images;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IImageQueryService
{
    Task<ImageResponseDto> GetImageByIdAsync(Guid id, CancellationToken cancellationToken);
}

