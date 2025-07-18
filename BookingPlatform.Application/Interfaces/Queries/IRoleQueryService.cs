using BookingPlatform.Application.Dtos.Roles;
using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IRoleQueryService
{
    Task<RoleResponseDto> GetRoleByIdAsync(Guid id, CancellationToken ct);
    Task<RoleResponseDto> GetRoleByTypeAsync(RoleType roleType, CancellationToken ct);

}

