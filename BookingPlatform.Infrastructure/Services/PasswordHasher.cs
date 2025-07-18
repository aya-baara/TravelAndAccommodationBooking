using BookingPlatform.Core.Interfaces.Services;
using Bcrypt = BCrypt.Net.BCrypt;

public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password) => Bcrypt.HashPassword(password);

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return Bcrypt.Verify(password, hashedPassword);
    }
}
