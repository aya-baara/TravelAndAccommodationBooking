using BookingPlatform.Core.Enums;

namespace BookingPlatform.Core.Entities;

public class Role : BaseEntity
{
    public RoleType Name { get; set; }
    public string Description { get; set; } = string.Empty;
    public List<User> Users { get; set; } = new List<User>();
}
