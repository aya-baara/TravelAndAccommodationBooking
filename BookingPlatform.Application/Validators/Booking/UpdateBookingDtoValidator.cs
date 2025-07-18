using BookingPlatform.Application.Dtos.Bookings;
using FluentValidation;

public class UpdateBookingDtoValidator : AbstractValidator<UpdateBookingDto>
{
    public UpdateBookingDtoValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Booking Id is required.");

        RuleFor(x => x.Remarks)
            .MaximumLength(500).WithMessage("Remarks cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Remarks));
    }
}
