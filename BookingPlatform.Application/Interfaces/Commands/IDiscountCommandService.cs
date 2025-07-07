using BookingPlatform.Application.Dtos.Discounts;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IDiscountCommandService
{
    Task<DiscountResponseDto> CreateDiscountAsync(CreateDiscountDto dto, CancellationToken cancellationToken);
    Task UpdateDiscountAsync(UpdateDiscountDto dto, CancellationToken cancellationToken);
    Task DeleteDiscountAsync(Guid id, CancellationToken cancellationToken);
}

