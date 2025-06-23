namespace BookingPlatform.Core.Interfaces;

public interface IPdfService
{
    byte[] GeneratePdfFromHtml(string htmlContent);
}

