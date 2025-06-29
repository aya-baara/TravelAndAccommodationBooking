using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class OwnerRepository : IOwnerRepository
{
    private readonly AppDbContext _context;
    public OwnerRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Owner> CreateOwnerAsync(Owner owner)
    {
        var result = await _context.Owners.AddAsync(owner);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task DeleteOwnerById(Guid ownerId)
    {
        var owner = await GetOwnerByIdAsync(ownerId);
        if (owner != null)
        {
            _context.Owners.Remove(owner);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<PaginatedResult<Hotel>> GetHotelsByOwnerIdAsync(Guid ownerId, int page, int size)
    {
        var totalCount = await _context.Hotels.Where(h => h.OwnerId == ownerId).CountAsync();

        var items = await _context.Hotels
            .Where(h => h.OwnerId == ownerId)
            .Skip((page - 1) * size)
            .Take(size)
            .AsNoTracking()
            .ToListAsync();
        return new PaginatedResult<Hotel>(items, totalCount, page, size);
    }

    public async Task<Owner?> GetOwnerByIdAsync(Guid ownerId)
    {
        return await _context.Owners.FirstOrDefaultAsync(o => o.Id == ownerId);
    }

    public async Task UpdateOwner(Owner owner)
    {
        _context.Owners.Update(owner);
        await _context.SaveChangesAsync();
    }
}

