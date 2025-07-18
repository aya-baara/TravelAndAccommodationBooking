using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Services;

public interface IInvoiceHtmlBuilder
{
    string BuildInvoiceDetailsHtml(Invoice invoice);
}

