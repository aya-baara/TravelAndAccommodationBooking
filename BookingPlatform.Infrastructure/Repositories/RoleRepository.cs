using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Enums;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly AppDbContext _context;
    public RoleRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Role> CreateRoleAsync(Role role, CancellationToken cancellationToken = default)
    {
        var result = await _context.Roles.AddAsync(role, cancellationToken);
        return result.Entity;

    }

    public async Task DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId, cancellationToken);
        if (role != null)
        {
            _context.Roles.Remove(role);
        }
    }

    public async Task<List<User>> GetAdmins(CancellationToken cancellationToken = default)
    {
        return await _context.Users.Where(u => u.Role.Name == UserRole.Admin).ToListAsync(cancellationToken);
    }

    public async Task<Role> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Roles.FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Roles.ToListAsync(cancellationToken);
    }
}

