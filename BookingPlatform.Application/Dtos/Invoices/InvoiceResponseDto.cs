using BookingPlatform.Core.Enums;

namespace BookingPlatform.Application.Dtos.Invoices;

public class InvoiceResponseDto
{
    public Guid Id { get; set; }
    public Guid BookingId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public PaymentType PaymentMethod { get; set; }
    public bool IsPaid { get; set; }
    public string? Notes { get; set; }
}

