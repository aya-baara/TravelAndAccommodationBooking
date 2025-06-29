using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class DiscountRepository : IDiscountRepository
{
    private readonly AppDbContext _context;

    public DiscountRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Discount> CreateDiscountAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        var result = await _context.Discounts.AddAsync(discount, cancellationToken);
        return result.Entity;
    }

    public async Task DeleteDiscountByIdAsync(Guid discountId, CancellationToken cancellationToken = default)
    {
        var discount = await GetDiscountByIdAsync(discountId, cancellationToken);
        if (discount != null)
        {
            _context.Discounts.Remove(discount);
        }
    }

    public async Task<Discount?> GetDiscountByIdAsync(Guid discountId, CancellationToken cancellationToken = default)
    {
        return await _context.Discounts.FirstOrDefaultAsync(d => d.Id == discountId, cancellationToken);
    }

    public async Task<List<Discount>> GetDiscountByRoomIdAsync(Guid roomId, CancellationToken cancellationToken = default)
    {
        return await _context.Discounts.Where(d => d.RoomId == roomId).ToListAsync(cancellationToken);
    }

    public async Task<PaginatedResult<Discount>> GetDiscountsAsync(int page, int size, CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Discounts.CountAsync(cancellationToken);
        var items = await _context.Discounts
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<Discount>(items, totalCount, page, size);
    }

    public async Task UpdateDiscountAsync(Discount discount, CancellationToken cancellationToken = default)
    {
        _context.Discounts.Update(discount);
    }
}

