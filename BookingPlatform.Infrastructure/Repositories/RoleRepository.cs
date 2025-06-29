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

    public async Task<Role> CreateRoleAsync(Role role)
    {
        var result = await _context.Roles.AddAsync(role);
        await _context.SaveChangesAsync();
        return result.Entity;

    }

    public async Task DeleteRoleAsync(Guid roleId)
    {
        var role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);
        if(role != null)
        {
            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<User>> GetAdmins()
    {
        return await _context.Users.Where(u => u.Role.Name == UserRole.Admin).ToListAsync();
    }

    public async Task<List<Role>> GetRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }
}

