using BookingPlatform.Application.Dtos.Invoices;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Invoices;

public class UpdateInvoiceDtoValidator : AbstractValidator<UpdateInvoiceDto>
{
    public UpdateInvoiceDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Invoice ID is required.");

        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required.");

        RuleFor(x => x.InvoiceDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Invoice date cannot be in the future.");

        RuleFor(x => x.TotalAmount)
            .GreaterThan(0).WithMessage("Total amount must be greater than 0.");

        RuleFor(x => x.PaymentMethod)
            .IsInEnum().WithMessage("Invalid payment method.");

        RuleFor(x => x.Notes)
            .MaximumLength(500).WithMessage("Notes must not exceed 500 characters.");
    }
}
