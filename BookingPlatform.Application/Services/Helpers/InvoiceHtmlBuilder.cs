using BookingPlatform.Core.Entities;
using BookingPlatform.Core.Interfaces.Services;

namespace BookingPlatform.Application.Services.Helpers;

public class InvoiceHtmlBuilder : IInvoiceHtmlBuilder
{
    public string BuildInvoiceDetailsHtml(Invoice invoice)
    {
        var booking = invoice.Booking;
        return
            $@"
        <html>
            <head>
                <style>
                    body {{ font-family: Arial; }}
                    h1 {{ color: #2c3e50; }}
                    table {{ width: 100%; border-collapse: collapse; }}
                    th, td {{ border: 1px solid #ddd; padding: 8px; }}
                </style>
            </head>
            <body>
                <h1>Invoice</h1>
                <p><strong>Invoice ID:</strong> {invoice.Id}</p>
                <p><strong>Date:</strong> {invoice.InvoiceDate:yyyy-MM-dd}</p>
                <p><strong>Payment Method:</strong> {invoice.PaymentMethod}</p>
                <p><strong>Paid:</strong> {(invoice.IsPaid ? "Yes" : "No")}</p>
                <p><strong>Total Amount:</strong> ${invoice.TotalAmount:F2}</p>
                <h2>Booking Info</h2>
                <p><strong>Check In:</strong> {booking.CheckIn:yyyy-MM-dd}</p>
                <p><strong>Check Out:</strong> {booking.CheckOut:yyyy-MM-dd}</p>
                <p><strong>Total Before Discount:</strong> ${booking.TotalPriceBeforeDiscount:F2}</p>
                <p><strong>Total After Discount:</strong> ${booking.TotalPriceAfterDiscount:F2}</p>
            </body>
        </html>";
    }
}

