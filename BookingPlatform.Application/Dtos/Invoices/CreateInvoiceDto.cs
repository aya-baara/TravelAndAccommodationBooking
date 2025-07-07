using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Invoices;

public class CreateInvoiceDto
{
    public Guid BookingId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public PaymentType PaymentMethod { get; set; }
    public bool IsPaid { get; set; }
    public string? Notes { get; set; }
}

