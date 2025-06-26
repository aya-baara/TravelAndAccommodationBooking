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

    public async Task CreateInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Add(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteInvoiceAsync(Guid invoiceId)
    {
        var invoice = await GetInvoiceByIdAsync(invoiceId);
        if(invoice != null)
        {
            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Invoice?> GetInvoiceByBookingIdAsync(Guid bookingId)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.BookingId == bookingId);
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(Guid invoiceId)
    {
        return await _context.Invoices.FirstOrDefaultAsync(i => i.Id == invoiceId);
    }

    public async Task UpdateInvoiceAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
    }
}

