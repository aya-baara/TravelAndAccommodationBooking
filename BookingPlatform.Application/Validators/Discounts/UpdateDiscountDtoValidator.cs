using BookingPlatform.Application.Dtos.Discounts;
using FluentValidation;

namespace BookingPlatform.Application.Validators.Discounts;

public class UpdateDiscountDtoValidator : AbstractValidator<UpdateDiscountDto>
{
    public UpdateDiscountDtoValidator()
    {
        RuleFor(x => x.Id)
           .NotEmpty().WithMessage("Discount ID is required.");

        RuleFor(x => x.RoomId)
           .NotEmpty().WithMessage("Room ID is required.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Start date cannot be in the past.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");

        RuleFor(x => x.Percentage)
            .GreaterThan(0).WithMessage("Discount percentage must be greater than 0.")
            .LessThanOrEqualTo(100).WithMessage("Discount percentage must not exceed 100.");
    }
}