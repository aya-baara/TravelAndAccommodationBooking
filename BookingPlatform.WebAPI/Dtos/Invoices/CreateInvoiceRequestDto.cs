using BookingPlatform.Core.Enums;

namespace BookingPlatform.WebAPI.Dtos.Invoices;

public class CreateInvoiceRequestDto
{
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public PaymentType PaymentMethod { get; set; }
    public bool IsPaid { get; set; }
    public string? Notes { get; set; }
}

