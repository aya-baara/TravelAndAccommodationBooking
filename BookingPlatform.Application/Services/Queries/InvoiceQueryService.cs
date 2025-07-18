using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Application.Interfaces.Queries;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Queries;

public class InvoiceQueryService :IInvoiceQueryService
{
    private readonly IInvoiceRopsitory _invoiceRopsitory;
    private readonly IMapper _mapper;
    private readonly ILogger<InvoiceQueryService> _logger;
    private readonly IPdfService _pdfService;
    private readonly IInvoiceHtmlBuilder _invoiceHtmlBuilder;

    public InvoiceQueryService(IInvoiceRopsitory invoiceRopsitory
        , IMapper mapper
        , ILogger<InvoiceQueryService> logger
        , IPdfService pdfService
        ,IInvoiceHtmlBuilder invoiceHtmlBuilder)
    {
        _invoiceRopsitory = invoiceRopsitory;
        _mapper = mapper;
        _logger = logger;
        _pdfService = pdfService;
        _invoiceHtmlBuilder = invoiceHtmlBuilder;
    }

    public async Task<InvoiceResponseDto> GetInvoiceByBookingId(Guid bookingId,CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRopsitory.GetInvoiceByBookingIdAsync(bookingId, cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning($"invoice with Booking ID {bookingId} not found");
            throw new NotFoundException("The Requested invoice Not found");
        }

        _logger.LogInformation($"Successfully retrieved invoice with Booking ID {bookingId}");

        return _mapper.Map<InvoiceResponseDto>(invoice);
    }

    public async Task<byte[]> PrintInvoice(Guid bookingId, Guid userId,CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRopsitory.GetInvoiceByBookingIdAsync(bookingId, cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning($"Invoice with Booking ID {bookingId} not found");
            throw new NotFoundException("The requested invoice was not found.");
        }

        if (invoice.Booking.UserId != userId)
        {
            _logger.LogWarning($"Unauthorized attempt to access invoice {bookingId} by user {userId}");
            throw new ForbiddenAccessException("You are not allowed to access this invoice.");
        }

        var booking = invoice.Booking;

        var htmlContent = _invoiceHtmlBuilder.BuildInvoiceDetailsHtml(invoice) ;

        var pdfBytes = _pdfService.GeneratePdfFromHtml(htmlContent);
        return pdfBytes;
    }
}

