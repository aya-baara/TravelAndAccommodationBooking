using BookingPlatform.Application.Dtos.Invoices;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IInvoiceQueryService
{
    Task<InvoiceResponseDto> GetInvoiceByBookingId(Guid id, CancellationToken cancellationToken);
    Task<byte[]> PrintInvoice(Guid id, Guid userId, CancellationToken cancellationToken);
}

