using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Models;

namespace BookingPlatform.Core.Interfaces.Auth;

public interface ITokenGenerator
{
    public AuthToken GenerateToken(User user);
    public bool ValidateToken(AuthToken token);
}

