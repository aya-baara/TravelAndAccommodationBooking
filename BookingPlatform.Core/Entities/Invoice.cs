using BookingPlatform.Core.Enums;

namespace BookingPlatform.Core.Entities;

public class Invoice : BaseEntity
{
    public Guid BookingId { get; set; }
    public Booking Booking { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public PaymentType PaymentMethod { get; set; }
    public bool IsPaid { get; set; }
    public string? Notes { get; set; }
}

