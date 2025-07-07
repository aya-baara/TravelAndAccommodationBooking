using BookingPlatform.Application.Dtos.Invoices;

namespace BookingPlatform.Application.Interfaces.Commands;

public interface IInvoiceCommandService
{
    Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken);
    Task DeleteInvoiceAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateInvoiceAsync(UpdateInvoiceDto dto, CancellationToken cancellationToken);
}

