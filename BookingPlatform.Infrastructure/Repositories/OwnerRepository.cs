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

    public async Task<Owner> CreateOwnerAsync(Owner owner, CancellationToken cancellationToken = default)
    {
        var result = await _context.Owners.AddAsync(owner, cancellationToken);
        return result.Entity;
    }

    public async Task DeleteOwnerById(Guid ownerId, CancellationToken cancellationToken = default)
    {
        var owner = await GetOwnerByIdAsync(ownerId, cancellationToken);
        if (owner != null)
        {
            _context.Owners.Remove(owner);
        }
    }

    public async Task<Owner?> GetOwnerByIdAsync(Guid ownerId, CancellationToken cancellationToken = default)
    {
        return await _context.Owners.FirstOrDefaultAsync(o => o.Id == ownerId, cancellationToken);
    }

    public async Task UpdateOwner(Owner owner, CancellationToken cancellationToken = default)
    {
        _context.Owners.Update(owner);
    }
}

