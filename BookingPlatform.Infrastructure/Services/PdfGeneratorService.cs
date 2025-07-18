using BookingPlatform.Core.Interfaces.Services;
using NReco.PdfGenerator;

namespace BookingPlatform.Infrastructure.Services;

public class PdfGeneratorService : IPdfService
{
    public byte[] GeneratePdfFromHtml(string htmlContent)
    {
        var converter = new HtmlToPdfConverter();
        return  converter.GeneratePdf(htmlContent);
    }
}

