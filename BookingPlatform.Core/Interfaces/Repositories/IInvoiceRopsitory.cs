using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

interface IInvoiceRopsitory
{
    Task CreateInvoiceAsync(Invoice invoice);
    Task<Invoice> GetInvoiceByIdAsync(Guid invoiceId);
    Task<Invoice> GetInvoiceByBookingIdAsync(Guid bookingId);
    Task UpdateInvoiceAsync(Invoice invoice);
    Task DeleteInvoiceAsync(Guid invoiceId);
}


