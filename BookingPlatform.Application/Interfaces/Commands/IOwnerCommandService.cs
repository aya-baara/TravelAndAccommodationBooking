using BookingPlatform.Application.Dtos.Owners;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IOwnerCommandService
{
    Task<OwnerResponseDto> CreateOwnerAsync(CreateOwnerDto dto, CancellationToken cancellationToken);
    Task UpdateOwnerAsync(UpdateOwnerDto dto, CancellationToken cancellationToken);
    Task DeleteOwnerAsync(Guid id, CancellationToken cancellationToken);
}

