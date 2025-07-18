namespace BookingPlatform.Core.Exceptions;

public class CredentialsNotValidException : Exception
{
    public CredentialsNotValidException(string message)
        : base(message)
    {
    }
}
