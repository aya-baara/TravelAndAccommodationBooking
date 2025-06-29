using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IOwnerRepository
{
    Task<Owner> CreateOwnerAsync(Owner owner, CancellationToken cancellationToken = default);
    Task<Owner?> GetOwnerByIdAsync(Guid ownerId, CancellationToken cancellationToken = default);
    Task UpdateOwner(Owner owner, CancellationToken cancellationToken = default);
    Task DeleteOwnerById(Guid ownerId, CancellationToken cancellationToken = default);
}

