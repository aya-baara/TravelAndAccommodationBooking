using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Roles;

public class CreateRoleDto
{
    public RoleType Name { get; set; }
    public string Description { get; set; } = string.Empty;
}

