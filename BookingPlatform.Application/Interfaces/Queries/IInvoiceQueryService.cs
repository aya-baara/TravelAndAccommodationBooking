using BookingPlatform.Application.Dtos.Invoices;

namespace BookingPlatform.Application.Interfaces.Queries;

public interface IInvoiceQueryService
{
    Task<InvoiceResponseDto> GetInvoiceById(Guid id, Guid userId, CancellationToken cancellationToken);
    Task<byte[]> PrintInvoice(Guid id, Guid userId,CancellationToken cancellationToken);
}

