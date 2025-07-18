namespace BookingPlatform.Core.Interfaces.Services;

public interface IPdfService
{
    byte[] GeneratePdfFromHtml(string htmlContent);
}

