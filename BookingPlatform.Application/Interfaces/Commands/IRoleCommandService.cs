using BookingPlatform.Application.Dtos.Roles;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IRoleCommandService
{
    Task<RoleResponseDto> CreateRoleAsync(CreateRoleDto dto , CancellationToken ct);
    Task DeleteRoleAsync(Guid id, CancellationToken ct);
}

