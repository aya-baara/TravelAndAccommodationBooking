using BookingPlatform.Application.Dtos.Invoices;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IInvoiceQueryService
{
    Task<InvoiceResponseDto> GetInvoiceById(Guid id, CancellationToken cancellationToken);
    Task<byte[]> PrintInvoice(Guid id, CancellationToken cancellationToken);
}

