using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> AuthenticateUserAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<User> CreateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User?> GetUserByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> DoesUserExistAsync(string email, CancellationToken cancellationToken = default);
}

