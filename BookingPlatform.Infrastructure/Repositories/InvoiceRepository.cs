using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Repositories;
using BookingPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BookingPlatform.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRopsitory
{
    private readonly AppDbContext _context;
    public InvoiceRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice> CreateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        var result = await _context.Invoices.AddAsync(invoice, cancellationToken);
        return result.Entity;
    }

    public async Task DeleteInvoiceAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId, cancellationToken);
        if (invoice != null)
        {
            _context.Invoices.Remove(invoice);
        }
    }

    public async Task<Invoice?> GetInvoiceByBookingIdAsync(Guid bookingId
        , CancellationToken cancellationToken = default)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.BookingId == bookingId, cancellationToken);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);
    }

    public async Task UpdateInvoiceAsync(Invoice invoice, CancellationToken cancellationToken = default)
    {
        _context.Invoices.Update(invoice);
    }
}

