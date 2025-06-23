using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IOwnerRepository
{
    Task CreateOwnerAsync(Owner owner);
    Task<Owner?> GetOwnerByIdAsync(Guid ownerId);
    Task<PaginatedResult<Hotel>> GetHotelsByOwnerIdAsync(Guid ownerId, int page, int size);
    Task UpdateOwner(Owner owner);
    Task DeleteOwnerById(Guid ownerId);
}

