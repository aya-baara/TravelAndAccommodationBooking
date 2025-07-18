using BookingPlatform.Application.Dtos.Images;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IImageCommandService
{
    Task<ImageResponseDto> CreateImageAsync(CreateImageDto dto, CancellationToken cancellationToken);
    Task DeleteImageAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateImageAsync(UpdateImageDto dto, CancellationToken cancellationToken);
}

