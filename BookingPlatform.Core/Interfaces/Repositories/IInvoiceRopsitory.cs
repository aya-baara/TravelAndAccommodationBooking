using BookingPlatform.Core.Entities;

namespace BookingPlatform.Core.Interfaces.Repositories;

public interface IInvoiceRopsitory
{
    Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default);
    Task<Invoice?> GetInvoiceByBookingIdAsync(Guid bookingId, CancellationToken cancellationToken = default);
    Task UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}


