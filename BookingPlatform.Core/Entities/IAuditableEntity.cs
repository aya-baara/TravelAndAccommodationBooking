namespace BookingPlatform.Core.Entities;

public interface IAuditableEntity
{
    DateTime CreatedAt { get; set; }
    DateTime ModifiedAt { get; set; }
}

