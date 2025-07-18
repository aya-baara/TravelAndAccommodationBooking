using BookingPlatform.Application.Dtos.Discounts;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IDiscountQueryService
{
    Task<DiscountResponseDto> GetDiscountByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<List<DiscountResponseDto>> GetDiscountsByRoom(Guid roomId, CancellationToken cancellationToken);
}

