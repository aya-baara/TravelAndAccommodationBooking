using BookingPlatform.Application.Dtos.Owners;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IOwnerQueryService
{
    Task<OwnerResponseDto> GetOwnerByIdAsync(Guid id, CancellationToken cancellationToken);
}

