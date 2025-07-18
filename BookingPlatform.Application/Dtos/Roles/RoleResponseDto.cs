using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Roles;

public class RoleResponseDto
{
    public Guid Id { get; set; }
    public RoleType Name { get; set; }
    public string Description { get; set; } = string.Empty;
}

