using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IRoleRepository
{
    Task<Role> CreateRoleAsync(Role role, CancellationToken cancellationToken = default);
    Task<List<Role>> GetRolesAsync(CancellationToken cancellationToken = default);
    Task<List<User>> GetAdmins(CancellationToken cancellationToken = default);
    Task DeleteRoleAsync(Guid roleId, CancellationToken cancellationToken = default);
}

