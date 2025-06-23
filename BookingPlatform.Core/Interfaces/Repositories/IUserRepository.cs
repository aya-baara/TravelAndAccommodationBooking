using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IUserRepository
{
    Task<User?> AuthenticateUserAsync(string email, string password);
    Task CreateUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<bool> DoesUserExistAsync(string email);
}

