namespace BookingPlatform.Core.Interfaces.Services;

interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
}

