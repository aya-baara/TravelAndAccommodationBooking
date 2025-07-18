using AutoMapper;
using BookingPlatform.Application.Dtos.Invoices;
using BookingPlatform.Application.Interfaces.Commands;
using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Exceptions;
using BookingPlatform.Core.Interfaces;
using BookingPlatform.Core.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace BookingPlatform.Application.Services.Commands;

public class InvoiceCommandService : IInvoiceCommandService
{
    private readonly IInvoiceRopsitory _invoiceRopsitory;
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<InvoiceCommandService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public InvoiceCommandService(IInvoiceRopsitory invoiceRopsitory
        , IBookingRepository bookingRepository
        , IMapper mapper, ILogger<InvoiceCommandService> logger
        , IUnitOfWork unitOfWork)
    {
        _invoiceRopsitory = invoiceRopsitory;
        _bookingRepository = bookingRepository;
        _mapper = mapper;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    public async Task<InvoiceResponseDto> CreateInvoiceAsync(CreateInvoiceDto dto, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetBookingById(dto.BookingId, cancellationToken);
        if (booking is null)
        {
            _logger.LogWarning($"Attempted to Add Invoice to non-existent Booking with ID {dto.BookingId}");
            throw new NotFoundException("The Requested Booking Not found");
        }
        var invoice = _mapper.Map<Invoice>(dto);
        invoice.TotalAmount = booking.TotalPriceBeforeDiscount;
        var created = await _invoiceRopsitory.CreateInvoiceAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync();
        var createdWithBooking = await _invoiceRopsitory.GetInvoiceByBookingIdAsync(created.BookingId, cancellationToken);


        _logger.LogInformation($"Invoice Created successfully with ID {created.Id}");

        return _mapper.Map<InvoiceResponseDto>(created);
    }

    public async Task DeleteInvoiceAsync(Guid bookingId, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRopsitory.GetInvoiceByBookingIdAsync(bookingId, cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Invoice with booking id {bookingId}");
            throw new NotFoundException("The Requested Invoice Not found");
        }
        await _invoiceRopsitory.DeleteInvoiceAsync(bookingId, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Invoice Deleted successfully with booking ID {bookingId}");
    }

    public async Task UpdateInvoiceAsync(UpdateInvoiceDto dto, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRopsitory.GetInvoiceByBookingIdAsync(dto.BookingId, cancellationToken);
        if (invoice is null)
        {
            _logger.LogWarning($"Attempted to Delete non-existent Invoice {dto.Id}");
            throw new NotFoundException("The Requested Invoice Not found");
        }
        var updated=_mapper.Map<Invoice>(dto);
        updated.Id = invoice.Id;
        await _invoiceRopsitory.UpdateInvoiceAsync(updated, cancellationToken);
        await _unitOfWork.SaveChangesAsync();

        _logger.LogInformation($"Invoice updated successfully with ID {dto.Id}");
    }
}

