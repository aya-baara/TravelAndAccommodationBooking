using BookingPlatform.Application.Dtos.Roles;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IRoleQueryService
{
    Task<RoleResponseDto> GetRoleByIdAsync(Guid id, CancellationToken ct);
}

