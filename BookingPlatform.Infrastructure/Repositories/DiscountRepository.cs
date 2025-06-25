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

    public async Task CreateDiscountAsync(Discount discount)
    {
        _context.Discounts.Add(discount);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteDiscountByIdAsync(Guid discountId)
    {
        var discount = await GetDiscountByIdAsync(discountId);
        if (discount != null)
        {
            _context.Discounts.Remove(discount);
        }
        await _context.SaveChangesAsync();

    }

    public async Task<Discount?> GetDiscountByIdAsync(Guid discountId)
    {
        return await _context.Discounts.FirstOrDefaultAsync(d => d.Id == discountId);
    }

    public async Task<List<Discount>> GetDiscountByRoomIdAsync(Guid roomId)
    {
        return await _context.Discounts.Where(d => d.RoomId == roomId).ToListAsync();
    }

    public async Task<PaginatedResult<Discount>> GetDiscountsAsync(int page, int size)
    {
        var totalCount = await _context.Discounts.CountAsync();

        var items = await _context.Discounts
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync();

        return new PaginatedResult<Discount>(items, totalCount, page, size);
    }

    public async Task UpdateDiscountAsync(Discount discount)
    {
        _context.Discounts.Update(discount);
        await _context.SaveChangesAsync();
    }
}

