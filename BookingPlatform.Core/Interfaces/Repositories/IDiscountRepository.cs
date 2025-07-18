using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IDiscountRepository
{
    Task<Discount> CreateDiscountAsync(Discount discount, CancellationToken cancellationToken = default);
    Task<Discount?> GetDiscountByIdAsync(Guid discountId, CancellationToken cancellationToken = default);
    Task<List<Discount>> GetDiscountByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Discount>> GetDiscountsAsync(int page = 1, int size = 20
        , CancellationToken cancellationToken = default);
    Task UpdateDiscountAsync(Discount discount, CancellationToken cancellationToken = default);
    Task DeleteDiscountByIdAsync(Guid discountId, CancellationToken cancellationToken = default);
    Task<Discount?> GetValidDiscountForRoomAsync(Guid roomId, DateTime checkIn, DateTime checkOut, CancellationToken cancellationToken =default);

}

