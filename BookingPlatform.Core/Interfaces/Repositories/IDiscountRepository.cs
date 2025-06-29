using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IDiscountRepository
{
    Task<Discount> CreateDiscountAsync(Discount discount);
    Task<Discount?> GetDiscountByIdAsync(Guid discountId);
    Task<List<Discount>> GetDiscountByRoomIdAsync(Guid roomId);
    Task<PaginatedResult<Discount>> GetDiscountsAsync(int page, int size);
    Task UpdateDiscountAsync(Discount discount);
    Task DeleteDiscountByIdAsync(Guid discountId);

}

