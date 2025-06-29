using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role> CreateRoleAsync(Role role);
    Task<List<Role>> GetRolesAsync();
    Task<List<User>> GetAdmins();
    Task DeleteRoleAsync(Guid roleId);
}

